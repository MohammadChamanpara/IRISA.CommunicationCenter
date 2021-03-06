using IRISA.CommunicationCenter.Library.Loggers;
using System;
using System.Linq;
using System.Text;
using System.Xml;

namespace IRISA.CommunicationCenter.Library.Definitions
{
    public class FieldDefinition : NodeBase, IFieldDefinition
    {
        public int Size
        {
            get
            {
                int result;
                try
                {
                    result = int.Parse(Node.Attributes["size"].InnerText.Trim());
                }
                catch
                {
                    throw IrisaException.Create("سایز عددی برای تعریف فیلد {0} مشخص نشده است.", new object[]
                    {
                        Name
                    });
                }
                return result;
            }
        }

        public string Type
        {
            get
            {
                string result;
                try
                {
                    result = Node.Attributes["type"].InnerText.Trim();
                }
                catch
                {
                    throw IrisaException.Create("نوع برای تعریف فیلد {0} مشخص نشده است.", new object[]
                    {
                        Name
                    });
                }
                return result;
            }
        }

        public bool IsArray
        {
            get
            {
                return Type.ToLower() == "array";
            }
        }

        public FieldDefinition(XmlNode node) : base(node)
        {
        }

        public byte[] GetBytes(string value)
        {
            string text = Type.ToLower();
            if (text != null)
            {
                byte[] result;
                if (!(text == "string"))
                {
                    if (!(text == "boolean"))
                    {
                        int size;
                        if (text == "number")
                        {
                            size = Size;
                            switch (size)
                            {
                                case 1:
                                    {
                                        if (byte.TryParse(value, out byte b))
                                        {
                                            result = new byte[]
                                            {
                                        b
                                            };
                                            return result;
                                        }
                                        throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به عدد صحیح 1 بایتی نیست.", new object[]
                                        {
                                    Name,
                                    value
                                        });
                                    }
                                case 2:
                                    {
                                        if (short.TryParse(value, out short value2))
                                        {
                                            result = BitConverter.GetBytes(value2);
                                            return result;
                                        }
                                        throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به عدد صحیح 2 بایتی نیست.", new object[]
                                        {
                                    Name,
                                    value
                                        });
                                    }
                                case 3:
                                    break;
                                case 4:
                                    {
                                        if (int.TryParse(value, out int value3))
                                        {
                                            result = BitConverter.GetBytes(value3);
                                            return result;
                                        }
                                        throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به عدد صحیح 4 بایتی نیست.", new object[]
                                        {
                                    Name,
                                    value
                                        });
                                    }
                                default:
                                    if (size == 8)
                                    {
                                        if (long.TryParse(value, out long value4))
                                        {
                                            result = BitConverter.GetBytes(value4);
                                            return result;
                                        }
                                        throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به عدد صحیح 8 بایتی نیست.", new object[]
                                        {
                                        Name,
                                        value
                                        });
                                    }
                                    break;
                            }
                            throw CreateFieldTypeException();
                        }
                        if (!(text == "float"))
                        {
                            throw CreateFieldTypeException();
                        }
                        size = Size;
                        if (size != 4)
                        {
                            if (size != 8)
                            {
                                throw CreateFieldTypeException();
                            }

                            if (!double.TryParse(value, out double value5))
                            {
                                throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به عدد اعشاری 8 بایتی نیست.", new object[]
                                {
                                    Name,
                                    value
                                });
                            }
                            result = BitConverter.GetBytes(value5);
                        }
                        else
                        {
                            if (!float.TryParse(value, out float value6))
                            {
                                throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به عدد اعشاری 4 بایتی نیست.", new object[]
                                {
                                    Name,
                                    value
                                });
                            }
                            result = BitConverter.GetBytes(value6);
                        }
                    }
                    else
                    {
                        if (!bool.TryParse(value, out bool value7))
                        {
                            throw IrisaException.Create("محتوای فیلد {0} برابر با {1} می باشد و قابل تبدیل به مقدار بولین نیست.", new object[]
                            {
                                Name,
                                value
                            });
                        }
                        result = BitConverter.GetBytes(value7);
                    }
                }
                else
                {
                    if (value.Length > Size)
                    {
                        throw IrisaException.Create("طول رشته محتوای فیلد {0}  برابر با {1} و حداکثر طول مجاز {2} می باشد.", new object[]
                        {
                            Name,
                            value.Length,
                            Size
                        });
                    }
                    byte[] second = new byte[Size - value.Length];
                    byte[] bytes = Encoding.ASCII.GetBytes(value);
                    result = bytes.Concat(second).ToArray();
                }
                return result;
            }
            throw CreateFieldTypeException();
        }
        public string GetValue(byte[] fieldBytes)
        {
            if (fieldBytes.Length < Size)
            {
                throw IrisaException.Create("تعداد فیلد های ارسال شده کمتر از تعداد فیلد های تعریف شده می باشد.");
            }
            string text = Type.ToLower();
            if (text != null)
            {
                string result;
                if (!(text == "string"))
                {
                    if (!(text == "boolean"))
                    {
                        int size;
                        if (text == "number")
                        {
                            size = Size;
                            switch (size)
                            {
                                case 1:
                                    result = fieldBytes[0].ToString();
                                    return result;
                                case 2:
                                    result = BitConverter.ToInt16(fieldBytes, 0).ToString();
                                    return result;
                                case 3:
                                    break;
                                case 4:
                                    result = BitConverter.ToInt32(fieldBytes, 0).ToString();
                                    return result;
                                default:
                                    if (size == 8)
                                    {
                                        result = BitConverter.ToInt64(fieldBytes, 0).ToString();
                                        return result;
                                    }
                                    break;
                            }
                            throw CreateFieldTypeException();
                        }
                        if (!(text == "float"))
                        {
                            throw CreateFieldTypeException();
                        }
                        size = Size;
                        if (size != 4)
                        {
                            if (size != 8)
                            {
                                throw CreateFieldTypeException();
                            }
                            result = BitConverter.ToDouble(fieldBytes, 0).ToString();
                        }
                        else
                        {
                            result = BitConverter.ToSingle(fieldBytes, 0).ToString();
                        }
                    }
                    else
                    {
                        int size = Size;
                        if (size != 1)
                        {
                            throw CreateFieldTypeException();
                        }
                        result = BitConverter.ToBoolean(fieldBytes, 0).ToString();
                    }
                }
                else
                {
                    string text2 = Encoding.ASCII.GetString(fieldBytes, 0, Size);
                    int num = text2.IndexOf('\0');
                    if (num >= 0)
                    {
                        text2 = text2.Remove(num);
                    }
                    result = text2;
                }
                return result;
            }
            throw CreateFieldTypeException();
        }
        private Exception CreateFieldTypeException()
        {
            return IrisaException.Create("نوع داده {0} با سایز {1} در تعریف فیلد {2} صحیح نیست.", new object[]
            {
                Type,
                Size,
                Name
            });
        }
    }
}
