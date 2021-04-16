using System;
using AndroidDataRecorder.Misc;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace AndroidDataRecorder.Shared
{
    public class MarkerInputSelect<TValue> : InputSelect<TValue>
    {
        /// <summary>
        /// If value is marker use GetMarkerID instead of toString
        /// </summary>
        /// <param name="value"></param>
        /// <returns>MarkerID</returns>
        protected override string? FormatValueAsString(TValue? value)
        {
            if (typeof(TValue) == typeof(Marker))
            {
                return (value as Marker)?.GetMarkerId();

            }
            return base.FormatValueAsString(value);
        }
        
        /// <summary>
        /// Create Marker from markerID
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <param name="validationErrorMessage"></param>
        /// <returns>Marker</returns>
        protected override bool TryParseValueFromString(string? value, out TValue result, out string? validationErrorMessage)
        {
            if (typeof(TValue) == typeof(Marker))
            {
                Console.WriteLine("Custom Input Select");
                validationErrorMessage = null;
                result = (TValue)(object) MarkerList.Markers.Find(x => x.MarkerId.Equals(Convert.ToInt32(value)));

                return true;
            }
            return base.TryParseValueFromString(value, out result, out validationErrorMessage);
        }
    }
}