using System;
using System.ComponentModel;
using System.Globalization;

namespace AndroidDataRecorder.Misc
{
    [TypeConverter(typeof (MarkerConverter))]
    public class MarkerConverter : TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if( sourceType == typeof(string) )
                return true;
            else 
                return base.CanConvertFrom(context, sourceType);
        }
        
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if( value.GetType() == typeof(string))
            {
                return MarkerList.Markers.Find(x => x.markerId == Convert.ToInt32(value));
            }
            else
                return base.ConvertFrom(context, culture, value);
        }
        
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
        {
            if (destinationType == typeof(string)) 
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && 
                value is Marker tmp)
            {
                return tmp.timeStamp.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);   
        }
    }
}