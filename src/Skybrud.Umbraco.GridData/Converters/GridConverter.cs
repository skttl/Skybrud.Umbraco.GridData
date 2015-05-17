﻿using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Config;
using Skybrud.Umbraco.GridData.Extensions.Json;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Rendering;
using Skybrud.Umbraco.GridData.Values;

namespace Skybrud.Umbraco.GridData.Converters {

    /// <summary>
    /// Converter for handling the default editors (and their values and configs) of Umbraco.
    /// </summary>
    public class GridConverter : IGridConverter {

        public virtual bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            
            value = null;

            switch (control.Editor.Alias) {

                case "media":
                    value = GridControlMediaValue.Parse(control, token as JObject);
                    break;

                case "embed":
                    value = GridControlEmbedValue.Parse(control, token as JObject);
                    break;

                case "rte":
                    value = GridControlRichTextValue.Parse(control, token);
                    break;

                case "macro":
                    value = GridControlMacroValue.Parse(control, token as JObject);
                    break;

                case "headline":
                case "quote":
                    value = GridControlTextValue.Parse(control, token);
                    break;

            }
            
            return value != null;
        
        }

        public virtual bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig value) {
       
            value = null;

            switch (editor.Alias) {

                case "headline":
                case "quote":
                    value = GridEditorTextConfig.Parse(editor, token as JObject);
                    break;

            }

            return value != null;
        
        }

        public virtual bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {

            wrapper = null;
            
            switch (control.Editor.Alias) {

                case "media":
                    wrapper = control.GetControlWrapper<GridControlMediaValue>();
                    break;

                case "embed":
                    wrapper = control.GetControlWrapper<GridControlEmbedValue>();
                    break;

                case "rte":
                    wrapper = control.GetControlWrapper<GridControlRichTextValue>();
                    break;

                case "macro":
                    wrapper = control.GetControlWrapper<GridControlMacroValue>();
                    break;

                case "quote":
                case "headline":
                    wrapper = control.GetControlWrapper<GridControlTextValue, GridEditorTextConfig>();
                    break;

            }

            return wrapper != null;

        }
    
    }

}