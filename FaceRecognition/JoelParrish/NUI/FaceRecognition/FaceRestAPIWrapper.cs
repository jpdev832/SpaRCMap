using System;
using JoelParrish.NUI.FaceRecognition.utils;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

namespace JoelParrish.NUI.FaceRecognition
{
    public class FaceRestAPIWrapper : IFaceRecognition
    {
        public string roamingPath;
        public string nameSpace;

        private FaceRestAPI fra;
        private string apiKey;
        private string apiSecret;
        private string fb_user;
        private string fb_oauth_token;
        private string password;

        public FaceRestAPIWrapper(string apiKey, string apiSecret, string nameSpace, string tempFolder)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.fb_user = null;
            this.fb_oauth_token = null;
            this.password = null;
            this.nameSpace = nameSpace;

            fra = new FaceRestAPI(apiKey, apiSecret, null, false, "json", null, null);

            roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                Path.DirectorySeparatorChar + "SpaRCMap" +
                (tempFolder == null ? "" : (Path.DirectorySeparatorChar + tempFolder));

            if (!Directory.Exists(roamingPath))
                Directory.CreateDirectory(roamingPath);
        }
        public FaceRestAPIWrapper(string apiKey, string apiSecret, string password, string fb_user, string fb_oauth_token, string nameSpace, string tempFolder)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.fb_user = fb_user;
            this.fb_oauth_token = fb_oauth_token;
            this.password = password;
            this.nameSpace = nameSpace;

            fra = new FaceRestAPI(apiKey, apiSecret, password, false, "json", fb_user, fb_oauth_token);

            roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                Path.DirectorySeparatorChar + "SpaRCMap" +
                (tempFolder == null ? "" : (Path.DirectorySeparatorChar + tempFolder));

            if (!Directory.Exists(roamingPath))
                Directory.CreateDirectory(roamingPath);
        }

        #region IFaceRecognition Members

        public event EventHandler detected;

        public event EventHandler recognized;

        /// <summary>
        /// Delete directory containing temporary images
        /// </summary>
        public void cleanup()
        {
            Directory.Delete(roamingPath, true);
        }

        /// <summary>
        /// Takes object as a string representing namespace for face.com REST API
        /// </summary>
        /// <param name="extras">namespace</param>
        public void putExtras(object extras)
        {
            nameSpace = (string)extras;
        }

        public IFaceRecognition getNewInstance(object tempFolder)
        {
            return new FaceRestAPIWrapper(apiKey, apiSecret, password, fb_user, fb_oauth_token, nameSpace, (string)tempFolder);
        }

        public void detect(BitmapSource bmp, object extra)
        {
            FaceRestAPI.FaceAPI f = fra.faces_detect(null, createTempImage(bmp), null, null);
            //need to change to hold data
            EventArgs e = new FaceRecognitionEventArgs(f);
            detected(this, e);
        }

        public void recognize(BitmapSource bmp, object extra)
        {
            //can use database to store tempimage path so image can later be used
            //can generate possible ids based on area context
            List<string> uids = new List<string>();
            uids.Add("all");
            FaceRestAPI.FaceAPI f = fra.faces_recognize(null, uids, nameSpace, null, createTempImage(bmp), null, null);

            EventArgs e = new FaceRecognitionEventArgs(f);
            recognized(this, e);
        }

        #endregion

        //need to create try catch
        protected string createTempImage(BitmapSource bmp)
        {
            string path = roamingPath + Path.DirectorySeparatorChar + Path.GetRandomFileName() + ".jpg";

            FileStream f = new FileStream(path, FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(f);
            f.Close();

            Debug.WriteLine("Temp image path: " + path);
            return path;
        }
    }
}
