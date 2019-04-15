using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

using CustomControl;
using ParameterManager;
using LogMessageManager;
using HistoryManager;

namespace KPVisionInspectionFramework
{
    public partial class ucMainResultID : UserControl
    {
        private uint CodeErrCount = 0;
        private uint BlankErrCount = 0;
        private uint MixErrCount = 0;

        #region Count & Yield Variable
        private uint TotalCount
        {
            set { SegmentValueInvoke(SevenSegTotal, value.ToString()); }
            get { return Convert.ToUInt32(SevenSegTotal.Value); }
        }

        private uint GoodCount
        {
            set { SegmentValueInvoke(SevenSegGood, value.ToString()); }
            get { return Convert.ToUInt32(SevenSegGood.Value); }
        }

        private uint NgCount
        {
            set { SegmentValueInvoke(SevenSegNg, value.ToString()); }
            get { return Convert.ToUInt32(SevenSegNg.Value); }
        }

        private uint InputDummyCount
        {
            set { SegmentValueInvoke(SevenSegInputDummy, value.ToString()); }
            get { return Convert.ToUInt32(SevenSegInputDummy.Value); }
        }

        private uint OriginDummyCount
        {
            set { SegmentValueInvoke(SevenSegOriginDummy, value.ToString()); }
            get { return Convert.ToUInt32(SevenSegOriginDummy.Value); }
        }

        private double Yield
        {
            set { SegmentValueInvoke(SevenSegYield, value.ToString()); }
            get { return Convert.ToDouble(SevenSegYield.Value); }
        }

        private string NowLotSeparateNum;
        #endregion Count & Yield Variable

        #region Count & Yield Registry Variable
        private RegistryKey RegTotalCount;
        private RegistryKey RegGoodCount;
        private RegistryKey RegNgCount;
        private RegistryKey RegInputDummyCount;
        private RegistryKey RegOriginDummyCount;
        private RegistryKey RegYield;
        private RegistryKey RegNowLotSeparateNum;

        private string RegTotalCountPath = String.Format(@"KPVision\ResultCount\TotalCount");
        private string RegGoodCountPath = String.Format(@"KPVision\ResultCount\GoodCount");
        private string RegNgCountPath = String.Format(@"KPVision\ResultCount\NgCount");
        private string RegInputDummyCountPath = String.Format(@"KPVision\ResultCount\DummyCount");
        private string RegOriginDummyCountPath = String.Format(@"KPVision\ResultCount\OriginDummyCount");
        private string RegYieldPath = String.Format(@"KPVision\ResultCount\Yield");
        private string RegNowLotSeparateNumPath = String.Format(@"KPVision\LOTNum\NowLotSeparateNum");
        #endregion Count & Yield Registry Variable

        bool AutoModeFlag = false;

        //LDH, 2018.08.13, History 입력용 string 
        string[] HistoryParam;
        string LastRecipeName;
        string LastResult;

        //LDH, 2018.09.28, InData용
        string BackupFolderPath;
        string InDataFolderPath;
        string OutDataFolderPath;
        List<string> InDataList = new List<string>();
        List<string> OutDataList = new List<string>();

        int InDataLimitCount = 0;
        string LOTType;
        string NowLotNum;
        //string NowLotSeparateNum;
        string OperatorName;
        string MCNum;
        string NowInspectionID;
        string InspectionTime;
        string IndataTotalCount;
        
        public delegate void ScreenshotHandler(string ScreenshotImagePath);
        public event ScreenshotHandler ScreenshotEvent;

        public delegate void ReadLOTNumHandler(string LOTNum);
        public event ReadLOTNumHandler ReadLOTNumEvent;

        public delegate bool ShowOpenDummyWindowHandler();
        public event ShowOpenDummyWindowHandler ShowDummyWindowEvent;

        #region Initialize & DeInitialize
        public ucMainResultID(string _LastRecipeName)
        {
            BackupFolderPath = @"D:\VisionInspectionData\KPAirBlowerInspection\Log\";

            SetLastRecipeName(_LastRecipeName);

            InitializeComponent();
            this.Location = new Point(1, 1);

            RegTotalCount = Registry.CurrentUser.CreateSubKey(RegTotalCountPath);
            RegGoodCount = Registry.CurrentUser.CreateSubKey(RegGoodCountPath);
            RegNgCount = Registry.CurrentUser.CreateSubKey(RegNgCountPath);
            RegInputDummyCount = Registry.CurrentUser.CreateSubKey(RegInputDummyCountPath);
            RegOriginDummyCount = Registry.CurrentUser.CreateSubKey(RegOriginDummyCountPath);
            RegYield = Registry.CurrentUser.CreateSubKey(RegYieldPath);
            RegNowLotSeparateNum = Registry.CurrentUser.CreateSubKey(RegNowLotSeparateNumPath);
        }

        public void DeInitialize()
        {

        }
        
        private void LoadResultCount()
        {
            //TotalCount  = (Convert.ToUInt32(RegTotalCount.GetValue("Value")) != null) ? Convert.ToUInt32(RegTotalCount.GetValue("Value") : 0;
            TotalCount = Convert.ToUInt32(RegTotalCount.GetValue("Value"));
            GoodCount = Convert.ToUInt32(RegGoodCount.GetValue("Value"));
            NgCount = Convert.ToUInt32(RegNgCount.GetValue("Value"));
            InputDummyCount = Convert.ToUInt32(RegInputDummyCount.GetValue("Value"));
            OriginDummyCount = Convert.ToUInt32(RegOriginDummyCount.GetValue("Value"));
            Yield = Convert.ToDouble(RegYield.GetValue("Value"));
            NowLotSeparateNum = Convert.ToString(RegNowLotSeparateNum.GetValue("Value"));

            CLogManager.AddSystemLog(CLogManager.LOG_TYPE.INFO, "Load Result Count");
            CLogManager.AddSystemLog(CLogManager.LOG_TYPE.INFO, String.Format("TotalCount : {0}, GoodCount : {1}, NgCount : {2}, InputDummyCount : {3}, OriginDummyCount : {4}, Yield : {5:F3}", TotalCount, GoodCount, NgCount, InputDummyCount, OriginDummyCount, Yield));
        }

        private void SaveResultCount()
        {
            RegTotalCount.SetValue("Value", TotalCount, RegistryValueKind.String);
            RegGoodCount.SetValue("Value", GoodCount, RegistryValueKind.String);
            RegNgCount.SetValue("Value", NgCount, RegistryValueKind.String);
            RegInputDummyCount.SetValue("Value", InputDummyCount, RegistryValueKind.String);
            RegOriginDummyCount.SetValue("Value", OriginDummyCount, RegistryValueKind.String);
            RegYield.SetValue("Value", Yield, RegistryValueKind.String);
            RegNowLotSeparateNum.SetValue("Value", NowLotSeparateNum, RegistryValueKind.String);

            CLogManager.AddSystemLog(CLogManager.LOG_TYPE.INFO, "Save Result Count");
            CLogManager.AddSystemLog(CLogManager.LOG_TYPE.INFO, String.Format("TotalCount : {0}, GoodCount : {1}, NgCount : {2}, InputDummyCount : {3}, OriginDummyCount : {4}, Yield : {5:F3}, SepNum : {6}", TotalCount, GoodCount, NgCount, InputDummyCount, OriginDummyCount, Yield, NowLotSeparateNum));
        }
        #endregion Initialize & DeInitialize

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics, this.panelMain.ClientRectangle, Color.Green, ButtonBorderStyle.Solid);
        }
        
        private void gradientLabelDummyCount_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowDummyWindowEvent();
        }

        public void SetLastRecipeName(string _LastRecipeName)
        {
            LastRecipeName = _LastRecipeName;
        }

        //LDH, 2018.10.12, AutoMode 관리
        public void SetAutoMode(bool _AutoModeFlag)
        {
            AutoModeFlag = _AutoModeFlag;
        }

        //LDH, 2018.10.29, LOT Start 시 처리
        public void LOTStart(string[] _LOTInfo)
        {
            LOTType = _LOTInfo[0];
            NowLotSeparateNum = _LOTInfo[2];
            OperatorName = _LOTInfo[4];
            MCNum = _LOTInfo[5];
            InDataLimitCount = Convert.ToInt32(_LOTInfo[6]);

            //LDH, 201811.19, 분할 LOT Num으로 읽어오기
            //if (NowLotNum != "") ReadInData();
            if (NowLotSeparateNum != "") ReadInData();

            //LDH, 2018.10.29, RELOAD면 Backup Load
            if (LOTType == "@N") ReadBackupData();

            ReadLOTNumEvent(NowLotSeparateNum);
        }

        //LDH, 2018.10.11, LOT End 시 처리
        public void LOTEnd()
        {
            if (NowLotSeparateNum != "NO")
            {
                DateTime dateTime = DateTime.Now;

                OutDataList.Add("EOL");

                string FileName = String.Format("{0}_{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}.txt", NowLotSeparateNum, dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                string OutDataFilePath = String.Format("{0}\\{1}\\{2}", OutDataFolderPath, NowLotSeparateNum, FileName);

                //LDH, 2018.10.11, OutData\용, 작업자Log용 따로 저장
                WriteOutData(OutDataFilePath);
                WriteOutData(String.Format(@"{0}OutDataLog\{1:D4}\{2:D2}\{3:D2}\{4}", BackupFolderPath, dateTime.Year, dateTime.Month, dateTime.Day, FileName));

                File.Delete(String.Format(@"{0}BakcupLog\Output_Backup.txt", BackupFolderPath));
            }

            SaveResultCount();
            ReadLOTNumEvent("-");
        }

        //LDH, 2019.04.09, OriginDummyCount 설정
        public void SetOriginDummyCount(uint _OriginDummyCount)
        {
            OriginDummyCount = _OriginDummyCount;
            SegmentValueInvoke(SevenSegOriginDummy, OriginDummyCount.ToString());
        }

        //LDH, 2019.04.09, Dummy Count 확인
        public bool CheckDummyCount()
        {
            bool _Return = true;
            if (SevenSegInputDummy.Value != SevenSegOriginDummy.Value) _Return = false;

            return _Return;
        }

        //LDH, 2018.10.01, Result clear
        public void ClearResult(bool _DUMMYFlag, string _LotNum, string _InDataPath, string _OutDataPath)
        {
            TotalCount = 0;
            GoodCount = 0;
            NgCount = 0;            
            Yield = 0.0;

            if (_DUMMYFlag) { OriginDummyCount = 0; InputDummyCount = 0; }

            CodeErrCount = 0;
            BlankErrCount = 0;
            MixErrCount = 0;
            
            ControlInvoke.GradientLabelText(gradientLabelResult, "-", Color.Black);
            ControlInvoke.GradientLabelText(gradientLabelDataMatrix, "-", Color.Black);

            DateTime datetime = DateTime.Now;
            SegmentValueInvoke(SevenSegTotal, TotalCount.ToString());
            SegmentValueInvoke(SevenSegGood, GoodCount.ToString());
            SegmentValueInvoke(SevenSegNg, NgCount.ToString());
            SegmentValueInvoke(SevenSegInputDummy, InputDummyCount.ToString());
            SegmentValueInvoke(SevenSegOriginDummy, OriginDummyCount.ToString());
            SegmentValueInvoke(SevenSegYield, Yield.ToString());

            SegmentValueInvoke(SevenSegCodeErr, CodeErrCount.ToString());
            SegmentValueInvoke(SevenSegBlankErr, BlankErrCount.ToString());
            SegmentValueInvoke(SevenSegMixErr, MixErrCount.ToString());

            InDataList.Clear();
            OutDataList.Clear();

            InDataLimitCount = 0;
            LOTType = "";
            NowLotSeparateNum = "";
            OperatorName = "";
            MCNum = "";
            NowInspectionID = "";
            InspectionTime = "";

            //LDH, 2018.09.27, InData List
            NowLotNum = _LotNum;
            InDataFolderPath = _InDataPath;
            OutDataFolderPath = _OutDataPath;

            //LDH, 2018.08.13, Hitory Parameter용 배열 초기화
            HistoryParam = new string[4];
            for (int iLoopCount = 0; iLoopCount < HistoryParam.Count(); iLoopCount++)
            {
                HistoryParam[iLoopCount] = "-";
            }
        }

        public void SetResultData(SendResultParameter _ResultParam)
        {
            string _DataMatrixString = "-";
            var _Result = _ResultParam.SendResult as SendIDResult;

            if (_Result != null) _DataMatrixString = (_ResultParam.IsGood == true) ? _Result.ReadCode : "-----";
            else                _DataMatrixString = "-";

            ControlInvoke.GradientLabelText(gradientLabelDataMatrix, _DataMatrixString);

            DateTime dateTime = DateTime.Now;
            InspectionTime = String.Format("{0:D2}{1:D2}{2:D2}{3:D3}", dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

            if (_ResultParam.IsGood)
            {
                LastResult = "GOOD";

                if (eNgType.DUMMY == _ResultParam.NgType)
                {
                    LastResult = "DUMMY";
                }

                else if (_Result == null)
                {
                    if (NowLotSeparateNum != "NO")
                        LastResult = "CODE NG";
                }
   
                else
                {
                    if (NowLotSeparateNum != "NO")
                    {
                        NowInspectionID = _DataMatrixString;
                        LastResult = SetOutData();
                    }
                } 

                if (LastResult == "OK")         ControlInvoke.GradientLabelText(gradientLabelResult, LastResult, Color.Lime);
                else if (LastResult == "DUMMY") ControlInvoke.GradientLabelText(gradientLabelResult, LastResult, Color.Lime);
                else if (LastResult == "GOOD")  ControlInvoke.GradientLabelText(gradientLabelResult, LastResult, Color.Lime);
                else                            ControlInvoke.GradientLabelText(gradientLabelResult, LastResult, Color.Red);

                if (LastResult != "GOOD" && LastResult != "OK" && LastResult != "DUMMY") { _ResultParam.IsGood = false; _ResultParam.NgType = eNgType.ID; }

                if (AutoModeFlag)
                {
                    if (LastResult == "DUMMY")
                    {
                        InputDummyCount++;
                        SegmentValueInvoke(SevenSegInputDummy, InputDummyCount.ToString());
                    }
                    else
                    {
                        TotalCount++;
                        if (_ResultParam.IsGood) GoodCount++;
                        else NgCount++;

                        Yield = (double)GoodCount / (double)TotalCount * 100;

                        SegmentValueInvoke(SevenSegTotal, TotalCount.ToString());
                        SegmentValueInvoke(SevenSegGood, GoodCount.ToString());
                        SegmentValueInvoke(SevenSegNg, NgCount.ToString());
                        SegmentValueInvoke(SevenSegYield, Yield.ToString("F2"));
                    }
                }
            }

            else
            {
                if (AutoModeFlag)
                {
                    TotalCount++;
                    NgCount++;
                    Yield = (double)GoodCount / (double)TotalCount * 100;

                    SegmentValueInvoke(SevenSegTotal, TotalCount.ToString());
                    SegmentValueInvoke(SevenSegNg, NgCount.ToString());
                    SegmentValueInvoke(SevenSegYield, Yield.ToString("F2"));
                }

                //gradientLabelDataMatrix
                if (eNgType.ID == _ResultParam.NgType)
                {
                    if (AutoModeFlag)
                    {
                        CodeErrCount++;
                        SegmentValueInvoke(SevenSegCodeErr, CodeErrCount.ToString());
                    }
                    LastResult = "CODE NG";
                }

                else if (eNgType.EMPTY == _ResultParam.NgType)
                {
                    if (AutoModeFlag)
                    {
                        BlankErrCount++;
                        SegmentValueInvoke(SevenSegBlankErr, BlankErrCount.ToString());
                    }
                    LastResult = "EMPTY";
                }

                else if (eNgType.REF_NG == _ResultParam.NgType)
                {
                    if (AutoModeFlag)
                    {
                        MixErrCount++;
                        SegmentValueInvoke(SevenSegMixErr, MixErrCount.ToString());
                    }
                    LastResult = "MIX NG";
                }

                else
                {
                    LastResult = "CODE NG";
                }

                ControlInvoke.GradientLabelText(gradientLabelResult, LastResult, Color.Red);
            }

            this.Refresh();

            SaveResultCount();
            InspectionHistory(_DataMatrixString);

        }

        private void SegmentValueInvoke(DmitryBrant.CustomControls.SevenSegmentArray _Control, string _Value)
        {
            if (_Control.InvokeRequired)
            {
                _Control.Invoke(new MethodInvoker(delegate () { _Control.Value = _Value; }));
            }
            else
            {
                _Control.Value = _Value;
            }
        }

        //LDH, 2018.10.11, Backup용 OutData 읽어오기
        private void ReadBackupData()
        {
            FileManager objFileManager = new FileManager();
            List<string> FileReadTemp;

            string BackupFilePath = String.Format(@"{0}BakcupLog\Output_Backup.txt", BackupFolderPath);

            if (File.Exists(BackupFilePath))
            {
                FileReadTemp = objFileManager.txtFileRead(BackupFilePath);

                OutDataList.Clear();

                for (int iLoopCount = 0; iLoopCount < FileReadTemp.Count; iLoopCount++)
                {
                    if (FileReadTemp[0] != "") { NowLotSeparateNum = FileReadTemp[0].Substring(0, FileReadTemp[0].IndexOf(',')); ReadLOTNumEvent(NowLotSeparateNum); }
                    if (FileReadTemp[iLoopCount] != "") OutDataList.Add(FileReadTemp[iLoopCount]);
                }

                LoadResultCount();
            }
            else ReadLOTNumEvent("-");

            LoadResultCount();
        }

        //LDH, 2018.09.27, InData 읽어오기
        private void ReadInData()
        {
            FileManager objFileManager = new FileManager();
            List<string> FileReadTemp;

            DateTime datetime = DateTime.Now;

            //LDH, 2018.11.19, 분할 LOT로 file read 하도록 변경
            //string FolderPath = InDataFolderPath + NowLotNum;
            string FolderPath = InDataFolderPath + NowLotSeparateNum;

            if (NowLotSeparateNum != "NO")
            {
                try
                {
                    if (Directory.Exists(FolderPath))
                    {
                        DirectoryInfo DInfo = new DirectoryInfo(FolderPath);
                        List<string> FileList = new List<string>();

                        FileInfo[] Files = DInfo.GetFiles();
                        var HiddenFilter = Files.Where(ReadFiles => !ReadFiles.Attributes.HasFlag(FileAttributes.Hidden));

                        foreach (var item in HiddenFilter)
                        {
                            if(item.Extension == ".txt") FileList.Add(item.Name);
                        }

                        FileList.Sort();
                        FolderPath = FolderPath + "\\" + FileList[FileList.Count - 1];
                    }
                    else
                    {
                        MessageBox.Show("LOT InData 폴더가 없습니다."); return;
                    }
                }
                catch
                {
                    MessageBox.Show("LOT 폴더에 InData 파일이 없습니다."); return;
                }

                CLogManager.AddSystemLog(CLogManager.LOG_TYPE.INFO, String.Format("INFO : {0}", Path.GetFileNameWithoutExtension(FolderPath)), CLogManager.LOG_LEVEL.LOW);

                FileReadTemp = objFileManager.txtFileRead(FolderPath);

                OutDataList.Clear();

                for (int iLoopCount = 0; iLoopCount < FileReadTemp.Count; iLoopCount++)
                {
                    if (FileReadTemp[iLoopCount] != "" && FileReadTemp[iLoopCount] != "EOL")
                    {
                        InDataList.Add(FileReadTemp[iLoopCount]);
                        OutDataList.Add(FileReadTemp[iLoopCount]);
                    }
                }

                string[] TotalCountTemp = InDataList[0].Split(',');

                IndataTotalCount = TotalCountTemp[1];
            }
            else IndataTotalCount = "0";
        }

        public string GetTotalCount()
        {
            return IndataTotalCount;
        }

        //LDH, 2018.09.28, OutData 쓰기
        private void WriteOutData(string OutDataFilePath, bool BackupFlag = false)
        {
            FileManager objFileManager = new FileManager();

            List<string> OutDataListTemp = new List<string>();

            string[] DataCheck;
            bool WriteFlag = false;
            int GoodCount = 0;

            for (int iLoopCount = 0; iLoopCount < OutDataList.Count; iLoopCount++)
            {
                DataCheck = OutDataList[iLoopCount].Split(',');

                if (!BackupFlag)
                {
                    if (iLoopCount == 0 || iLoopCount == OutDataList.Count - 1) WriteFlag = true;
                    else if (DataCheck[12] != "") WriteFlag = true;
                }
                else WriteFlag = true;

                if (WriteFlag)
                {
                    OutDataListTemp.Add(OutDataList[iLoopCount]);

                    if (OutDataList[iLoopCount].Contains("OK")) GoodCount++;
                }

                WriteFlag = false;
            }

            if (OutDataList.Count != 0)
            {
                string[] FirstLineTemp = OutDataList[0].Split(',');

                FirstLineTemp[1] = GoodCount.ToString();
                OutDataListTemp[0] = String.Join(",", FirstLineTemp);
            }

            objFileManager.txtFileWrite(OutDataFilePath, OutDataListTemp);
        }

        //LDH, 2018.09.27, Outdata 추가F
        private string SetOutData()
        {
            int IDIndex = -1;
            string ErrorCode = "OK";
            string Result = "OK";

            string TempInData;
            string[] ArrInData = null;

            if (OutDataList.Count == 0) return "NG";

            try
            {
                IDIndex = OutDataList.FindIndex(FindText);

                TempInData = OutDataList[IDIndex];
                ArrInData = OutDataList[IDIndex].Split(',');

                //if (NowInspectionID.Contains(ArrInData[1]))
                if (NowInspectionID.Contains(NowLotNum))
                {
                    if (InDataLimitCount < Convert.ToInt32(ArrInData[2])) ErrorCode = "1";
                }
                else ErrorCode = "2";
            }
            catch
            {
                TempInData = OutDataList[0];

                string[] LOTInfo = TempInData.Split(',');

                if (NowInspectionID.Contains(LOTInfo[0])) ErrorCode = "3";
                else ErrorCode = "2";

                string[] AddListData = new string[14];
                AddListData[0] = NowInspectionID;
                AddListData[1] = NowInspectionID.Substring(0, NowInspectionID.IndexOf(' '));
                AddListData[2] = "1";
                AddListData[13] = "EOS";
                IDIndex = OutDataList.Count;

                ArrInData = AddListData;

                if (AutoModeFlag) OutDataList.Add("");
            }

            if (ErrorCode != "OK") Result = "NG";
            
            DateTime dateTime = DateTime.Now;
            InspectionTime = String.Format("{0:D2}{1:D2}{2:D2}{3:D3}", dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

            string OutDataTime = String.Format("{0:D4}{1:D2}{2:D2}{3}", dateTime.Year, dateTime.Month, dateTime.Day, InspectionTime.Substring(0,6));

            ArrInData[8] = NowLotSeparateNum;
            ArrInData[9] = OutDataTime;
            ArrInData[10] = TotalCount.ToString();
            ArrInData[11] = ErrorCode.ToString();
            ArrInData[12] = Result;

            if (AutoModeFlag)
            {
                OutDataList[IDIndex] = string.Join(",", ArrInData);

                WriteOutData(String.Format(@"{0}BakcupLog\Output_Backup.txt", BackupFolderPath), true);
            }

            if (ErrorCode != "OK")
            {
                if (ErrorCode == "1") Result = "Xout NG";
                else Result = "Indata NG";
            }

            return Result;
        }
        
        private bool FindText(string Text)
        {
            if (Text.Contains(NowInspectionID))
            {
                string[] TextTemp = Text.Split(',');

                if(TextTemp[0] == NowInspectionID) return true;
            }
            return false;
        }

        //LDH, 2018.08.13, History 추가용 함수
        private void InspectionHistory(string _ReadCodeData)
        {
            CLogManager.AddInspectionLog(CLogManager.LOG_TYPE.INFO, String.Format("InspectionHistory Start"), CLogManager.LOG_LEVEL.LOW);

            DateTime dateTime = DateTime.Now;
            string InspScreenshotPath = @"D:\VisionInspectionData\KPAirBlowerInspection\HistoryData\Screenshot\";
            string ImageSaveFolder = String.Format("{0}{1:D4}\\{2:D2}\\{3:D2}", InspScreenshotPath, dateTime.Year, dateTime.Month, dateTime.Day);

            if (false == Directory.Exists(ImageSaveFolder)) Directory.CreateDirectory(ImageSaveFolder);

            string ImageSaveFile;
            //ImageSaveFile = String.Format("{0}\\{1:D2}{2:D2}{3:D2}{4:D3}.bmp", ImageSaveFolder, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
            ImageSaveFile = String.Format("{0}\\{1}.jpg", ImageSaveFolder, InspectionTime);

            //LDH, 2018.08.13, 프로젝트별로 DB에 해당하는 history 내역을 string 배열로 전달
            HistoryParam[0] = LastRecipeName;
            HistoryParam[1] = LastResult;
            HistoryParam[2] = _ReadCodeData;
            HistoryParam[3] = ImageSaveFile;

            CHistoryManager.AddHistory(HistoryParam);
            CLogManager.AddInspectionLog(CLogManager.LOG_TYPE.INFO, String.Format("InspectionHistory End"), CLogManager.LOG_LEVEL.LOW);

            CLogManager.AddInspectionLog(CLogManager.LOG_TYPE.INFO, String.Format("Screenshot Start"), CLogManager.LOG_LEVEL.LOW);
            ScreenshotEvent(ImageSaveFile);
            CLogManager.AddInspectionLog(CLogManager.LOG_TYPE.INFO, String.Format("Screenshot End"), CLogManager.LOG_LEVEL.LOW);
        }
    }
}
