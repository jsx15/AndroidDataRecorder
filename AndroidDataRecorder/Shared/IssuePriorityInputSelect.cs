#nullable enable
using System;
using AndroidDataRecorder.Misc;
using Atlassian.Jira;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace AndroidDataRecorder.Shared
{
    public class IssuePriorityInputSelect<TValue> : InputSelect<TValue>
    {
        /// <summary>
        /// If value is marker use GetMarkerID instead of toString
        /// </summary>
        /// <param name="value"></param>
        /// <returns>MarkerID</returns>
        protected override string? FormatValueAsString(TValue? value)
        {
            if (typeof(TValue) == typeof(IssuePriority))
            {
                return (value as IssueType)?.Id;

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
            try
            {
                if (typeof(TValue) == typeof(IssuePriority))
                {
                    validationErrorMessage = null;
                    //Database.TableMarker data = new Database.TableMarker();
                    //result = (TValue) (object)data.GetList().Find(x => x.MarkerId.Equals(Convert.ToInt32(value)))!;

                    result = (TValue) (object) new IssuePriority(value);

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