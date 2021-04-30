#nullable enable
using System;
using AndroidDataRecorder.Misc;
using Atlassian.Jira;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace AndroidDataRecorder.Shared
{
    public class IssueTypeInputSelect<TValue> : InputSelect<TValue>
    {
        /// <summary>
        /// If value is marker use GetMarkerID instead of toString
        /// </summary>
        /// <param name="value"></param>
        /// <returns>MarkerID</returns>
        protected override string? FormatValueAsString(TValue? value)
        {
            if (typeof(TValue) == typeof(IssueType))
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
#pragma warning disable 8765
        protected override bool TryParseValueFromString(string? value, out TValue result, out string? validationErrorMessage)
#pragma warning restore 8765
        {
            try
            {
                if (typeof(TValue) == typeof(IssueType))
                {
                    validationErrorMessage = null;

                    result = (TValue) (object) new IssueType(value);

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