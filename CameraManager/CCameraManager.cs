using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

using ParameterManager; 

namespace CameraManager
{
    public class CCameraManager
    {
        public delegate void ImageGrabHandler(byte[] GrabImage);
        public event ImageGrabHandler ImageGrabEvent;

        public delegate void ImageGrabIntPtrHandler(IntPtr GrabImage);
        public event ImageGrabIntPtrHandler ImageGrabIntPtrEvent;

        private CEuresysManager objEuresysManager;
        private CEuresysIOTAManager objEuresysIOTAManager;
        private CBaslerManager objBaslerManager;

        private string CameraType;
        bool CamLiveFlag = false;

        public CCameraManager()
        {

        }

        public bool Initialize(int _ID, string _CamType, string _CamInfo)
        {
            bool _Result = true;

            CameraType = _CamType;

            //LDH, 2019.03.14, Common 폴더에서 Exposure time 읽기 추가
            int CamExposure = ReadExposure();

            if (CameraType == eCameraType.Euresys.ToString())
            {
                if (_ID == 0)
                {
                    objEuresysManager = new CEuresysManager(_CamInfo, CamExposure);
                    objEuresysManager.EuresysGrabEvent += new CEuresysManager.EuresysGrabHandler(ImageGrabEvent);
                }
            }

            else if(CameraType == eCameraType.EuresysIOTA.ToString())
            {
                if (_ID == 0)
                {
                    objEuresysIOTAManager = new CEuresysIOTAManager(CamExposure);
                    objEuresysIOTAManager.EuresysGrabEvent += new CEuresysIOTAManager.EuresysGrabHandler(ImageGrabEvent);
                }
            }

            else if (CameraType == eCameraType.BaslerGE.ToString())
            {
                objBaslerManager = new CBaslerManager();
                if (true == objBaslerManager.Initialize(_ID, _CamInfo))
                    objBaslerManager.BaslerGrabEvent += new CBaslerManager.BaslerGrabHandler(ImageGrabEvent);
                else
                    _Result = false;
            }

            return _Result;
        }

        public void DeInitialilze()
        {
            if (CameraType == eCameraType.Euresys.ToString())
            {
                objEuresysManager.EuresysGrabEvent -= new CEuresysManager.EuresysGrabHandler(ImageGrabEvent);
                objEuresysManager.DeInitialize();
            }

            else if (CameraType == eCameraType.EuresysIOTA.ToString())
            {
                objEuresysIOTAManager.EuresysGrabEvent -= new CEuresysIOTAManager.EuresysGrabHandler(ImageGrabEvent);
                objEuresysIOTAManager.DeInitialize();
            }

            else if (CameraType == eCameraType.BaslerGE.ToString())
            {
                objBaslerManager.BaslerGrabEvent -= new CBaslerManager.BaslerGrabHandler(ImageGrabEvent);
                objBaslerManager.DeInitialize();
            }
        }

        #region Read & Write XmlFile
        public int ReadExposure()
        {
            int _ExposureTime = 0;
            string _CamExposureFolderPath = @"D:\VisionInspectionData\Common\";

            DirectoryInfo _DirInfo = new DirectoryInfo(_CamExposureFolderPath);
            if (false == _DirInfo.Exists) { _DirInfo.Create(); System.Threading.Thread.Sleep(100); }

            string _ExposureInfoFileName = string.Format(@"{0}ExposureInformation.xml", _CamExposureFolderPath);
            if (false == File.Exists(_ExposureInfoFileName))
            {
                File.Create(_ExposureInfoFileName).Close();
                WriteExposure();
                System.Threading.Thread.Sleep(100);
            }

            XmlNodeList _XmlNodeList = GetNodeList(_ExposureInfoFileName);
            if (null == _XmlNodeList) return 1000;
            foreach (XmlNode _Node in _XmlNodeList)
            {
                if (null == _Node) return 1000;
                switch (_Node.Name)
                {
                    case "ExposureTime": _ExposureTime = Convert.ToInt32(_Node.InnerText); break;
                }
            }

            return _ExposureTime;
        }

        private XmlNodeList GetNodeList(string _XmlFilePath)
        {
            XmlNodeList _XmlNodeList = null;

            try
            {
                XmlDocument _XmlDocument = new XmlDocument();
                _XmlDocument.Load(_XmlFilePath);
                XmlElement _XmlRoot = _XmlDocument.DocumentElement;
                _XmlNodeList = _XmlRoot.ChildNodes;
            }

            catch
            {
                _XmlNodeList = null;
            }

            return _XmlNodeList;
        }

        private void WriteExposure()
        {
            string _CamExposureFolderPath = @"D:\VisionInspectionData\Common\";

            DirectoryInfo _DirInfo = new DirectoryInfo(_CamExposureFolderPath);
            if (false == _DirInfo.Exists) { _DirInfo.Create(); System.Threading.Thread.Sleep(100); }

            string _ExposureInfoFileName = string.Format(@"{0}ExposureInformation.xml", _CamExposureFolderPath);
            XmlTextWriter _XmlWriter = new XmlTextWriter(_ExposureInfoFileName, Encoding.Unicode);
            _XmlWriter.Formatting = Formatting.Indented;
            _XmlWriter.WriteStartDocument();
            _XmlWriter.WriteStartElement("ExposureInformation");
            {
                _XmlWriter.WriteElementString("ExposureTime", "1000");
            }
            _XmlWriter.WriteEndElement();
            _XmlWriter.WriteEndDocument();
            _XmlWriter.Close();
        }

        #endregion Read & Write XmlFile

        public void CamLive(bool _IsLive = false)
        {
            CamLiveFlag = !CamLiveFlag;
            if (CameraType == eCameraType.Euresys.ToString())           objEuresysManager.SetActive(_IsLive);
            else if (CameraType == eCameraType.EuresysIOTA.ToString())  objEuresysIOTAManager.SetActive(_IsLive);
            else if (CameraType == eCameraType.BaslerGE.ToString())     objBaslerManager.Continuous(_IsLive);
        }

        public void CameraGrab()
        {
            if (CameraType == eCameraType.BaslerGE.ToString()) objBaslerManager.OneShot();
        }
    }
}
