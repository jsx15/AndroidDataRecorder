#nullable enable
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
            return typeof(TValue) == typeof(Marker) ? (value as Marker)?.GetMarkerId() : base.FormatValueAsString(value);
        }
        
        /// <summary>
        /// Create Marker from markerID
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <param name="validationErrorMessage"></param>
        /// <returns>Marker</returns>
#pragma warning disable 8765
        protected override bool TryParseValueFromString(string? value, out TValue result, out string? validationErrorMessage)
#pragma warning restore 8765
        {
            try
            {
                if (typeof(TValue) == typeof(Marker))
                {
                    validationErrorMessage = null;
                    Database.TableMarker data = new Database.TableMarker();
                    result = (TValue) (object)data.GetList().Find(x => x.MarkerId.Equals(Convert.ToInt32(value)))!;

                    return true;
                }
            }
            catch (Exception)
            {
                return base.TryParseValueFromString(value, out result!, out validationErrorMessage);
            }

            return base.TryParseValueFromString(value, out result!, out validationErrorMessage);
        }
    }
}