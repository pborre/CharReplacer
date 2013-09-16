namespace EAI.BE.BizTalk.PipelineComponents
{
    using System;
    using System.IO;
    using System.Text;
    using System.Drawing;
    using System.Resources;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections;
    using System.ComponentModel;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Messaging;
    using System.Collections.Generic;
    using Winterdom.BizTalk.Samples.FixEncoding.Design;

using Microsoft.XLANGs.BaseTypes;
    
    
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("0add01b8-e8de-417a-99f7-73796b91cab4")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public class CharacterTranscoderComponent : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("EAI.BE.BizTalk.PipelineComponents.CharacterTranscoder", Assembly.GetExecutingAssembly());

        private static PropertyBase EncodingInProperty = new TRANSCODER.EncodingIn();
        private static PropertyBase EncodingOutProperty = new TRANSCODER.EncodingOut();
        private static readonly Dictionary<int, string> _encodingList;

        private Encoding _EncodingOut = Encoding.UTF8;
        private Encoding _EncodingIn = Encoding.UTF8;
        
        [Editor(typeof(EncodingTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding EncodingOut
        {
            get
            {
                return _EncodingOut;
            }
            set
            {
                _EncodingOut = value;
            }
        }

        [Editor(typeof(EncodingTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding EncodingIn
        {
            get
            {
                return _EncodingIn;
            }
            set
            {
                _EncodingIn = value;
            }
        }
        
        #region IBaseComponent members
        /// <summary>
        /// Name of the component
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get
            {
                return resourceManager.GetString("COMPONENTNAME.CharacterTranscoder", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// Version of the component
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get
            {
                return resourceManager.GetString("COMPONENTVERSION.CharacterTranscoder", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// Description of the component
        /// </summary>
        [Browsable(false)]
        public string Description
        {
            get
            {
                return resourceManager.GetString("COMPONENTDESCRIPTION.CharacterTranscoder", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        #endregion


        static CharacterTranscoderComponent()
        {
            EncodingInfo[] encodings = Encoding.GetEncodings();
            _encodingList = new Dictionary<int, string>();
            foreach (EncodingInfo ei in encodings)
            {
                _encodingList.Add(ei.CodePage, ei.Name);
            }
        }


        #region IPersistPropertyBag members
        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component
        /// </param>
        public void GetClassID(out System.Guid classid)
        {
            classid = new System.Guid("0add01b8-e8de-417a-99f7-73796b91cab4");
        }
        
        /// <summary>
        /// not implemented
        /// </summary>
        public void InitNew()
        {
        }
        
        /// <summary>
        /// Loads configuration properties for the component
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="errlog">Error status</param>
        public virtual void Load(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, int errlog)
        {
            object val = null;
            val = this.ReadPropertyBag(pb, "EncodingOut");
            if ((val != null))
            {
                this._EncodingOut = Encoding.GetEncoding((int)(val));
            }
            val = this.ReadPropertyBag(pb, "EncodingIn");
            if ((val != null))
            {
                this._EncodingIn = Encoding.GetEncoding((int)(val));
            }
        }
        
        /// <summary>
        /// Saves the current component configuration into the property bag
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="fClearDirty">not used</param>
        /// <param name="fSaveAllProperties">not used</param>
        public virtual void Save(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties)
        {
            this.WritePropertyBag(pb, "EncodingOut", this.EncodingOut.CodePage);
            this.WritePropertyBag(pb, "EncodingIn", this.EncodingIn.CodePage);
        }
        
        #region utility functionality
        /// <summary>
        /// Reads property value from property bag
        /// </summary>
        /// <param name="pb">Property bag</param>
        /// <param name="propName">Name of property</param>
        /// <returns>Value of the property</returns>
        private object ReadPropertyBag(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, string propName)
        {
            object val = null;
            try
            {
                pb.Read(propName, out val, 0);
            }
            catch (System.ArgumentException )
            {
                return val;
            }
            catch (System.Exception e)
            {
                throw new System.ApplicationException(e.Message);
            }
            return val;
        }
        
        /// <summary>
        /// Writes property values into a property bag.
        /// </summary>
        /// <param name="pb">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <param name="val">Value of property.</param>
        private void WritePropertyBag(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (System.Exception e)
            {
                throw new System.ApplicationException(e.Message);
            }
        }
        #endregion
        #endregion
        
        #region IComponentUI members
        /// <summary>
        /// Component icon to use in BizTalk Editor
        /// </summary>
        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return ((System.Drawing.Bitmap)(this.resourceManager.GetObject("COMPONENTICON", System.Globalization.CultureInfo.InvariantCulture))).GetHicon();
            }
        }
        
        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build 
        /// of a BizTalk project.
        /// </summary>
        /// <param name="obj">An Object containing the configuration properties.</param>
    /// <returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>
        public System.Collections.IEnumerator Validate(object obj)
        {
            // example implementation:
            // ArrayList errorList = new ArrayList();
            // errorList.Add("This is a compiler error");
            // return errorList.GetEnumerator();
            return null;
        }
        #endregion
        
        #region IComponent members
        /// <summary>
        /// Implements IComponent.Execute method.
        /// </summary>
        /// <param name="pc">Pipeline context</param>
        /// <param name="inmsg">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        /// the processing of the message in this pipeline component.
        /// </remarks>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            Encoding encodingIn = this._EncodingIn;
            Encoding encodingOut = this._EncodingOut;

            object val;
            val = (object)inmsg.Context.Read(EncodingInProperty.Name.Name, EncodingInProperty.Name.Namespace);
            if ((val != null))
            {
                encodingIn = Encoding.GetEncoding((string)val);
            }
            val = (object)inmsg.Context.Read(EncodingOutProperty.Name.Name, EncodingOutProperty.Name.Namespace);
            if ((val != null))
            {
                encodingOut = Encoding.GetEncoding((string)val);
            }
            inmsg.BodyPart.Data = new TranscodingStream(
                inmsg.BodyPart.GetOriginalDataStream(),
                encodingOut, encodingIn);

            return inmsg;
        }
        #endregion
    }
}
