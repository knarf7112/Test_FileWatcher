using System;
using System.Diagnostics;
using System.IO;
namespace Test_FileWatcher
{
    /// <summary>
    /// 檔案異動監控
    /// </summary>
    public class FileSystemListener:IDisposable
    {
        private FileSystemWatcher _fileWatcher;
        /// <summary>
        /// file change event
        /// </summary>
        public event FileSystemEventHandler FileChange
        { 
            add 
            {
                lock (this._fileWatcher)
                {
                    this._fileWatcher.Changed += value;
                }
            } 
            remove 
            {
                lock (this._fileWatcher)
                {
                    this._fileWatcher.Changed -= value;
                }
            } 
        }
        /// <summary>
        /// file create event 
        /// </summary>
        public event FileSystemEventHandler FileCreate
        {
            add
            {
                lock (this._fileWatcher)
                {
                    this._fileWatcher.Created += value;
                }
            }
            remove
            {
                lock (this._fileWatcher)
                {
                    this._fileWatcher.Created -= value;
                }
            }
        }
        /// <summary>
        /// file delete event
        /// </summary>
        public event FileSystemEventHandler FileDelete
        {
            add
            {
                lock (this._fileWatcher)
                {
                    this._fileWatcher.Deleted += value;
                }
            }
            remove
            {
                lock (this._fileWatcher)
                {
                    this._fileWatcher.Deleted -= value;
                }
            }
        }

        /// <summary>
        /// 監聽的完整目錄路徑
        /// </summary>
        public readonly string ListenPath;// { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listenPath">監聽的目錄</param>
        /// <param name="filter">監聽的檔案類型(ex:*.txt)</param>
        /// <param name="useDefaultEvent">是否加入預設的事件(測試用)</param>
        public FileSystemListener(string listenPath,string filter,bool useDefaultEvent = false)
        {
            this.ListenPath = listenPath;
            //1.選告FileSystemWatcher類別
            this._fileWatcher = new FileSystemWatcher();
            //2.設定要監控的資料夾
            this._fileWatcher.Path = listenPath;
            //3.是否要監控此資料夾內的資料夾目錄，這邊預設為true
            this._fileWatcher.IncludeSubdirectories = true;
            //4.設定處理時候的Buffer，預設為4096 個位元組
            this._fileWatcher.InternalBufferSize = 16384;
            //5.設定要監控的檔案類型，我們這邊剛好需要任何檔案都要監控(檔名.副檔名)
            this._fileWatcher.Filter = filter;// || "*.txt";
            //6.設定NotifyFilter屬性，讓該資料夾發生哪些變化時候，可以觸發相關的event。裡面可用的屬性在我們操作檔案中都會有的。
            this._fileWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                 NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            if (useDefaultEvent)
            {
                this.Set_DefaultEvent();
            }
            //8.設定EnableRaisingEvents屬性，這邊須設定為true，否則則無法動作
            this._fileWatcher.EnableRaisingEvents = true;
        }

        #region 測試用
        private void Set_DefaultEvent()
        {
            //7.設定該資料夾變化時候需要做的工作，基本上會有四種事件分別為建立,改變屬性,改變名稱,刪除這些動作
            this._fileWatcher.Created += _fileWatcher_Created;
            this._fileWatcher.Changed += _fileWatcher_Changed;
            this._fileWatcher.Deleted += _fileWatcher_Deleted;
            this._fileWatcher.Disposed += _fileWatcher_Disposed;
            this._fileWatcher.Error += _fileWatcher_Error;
            this._fileWatcher.Renamed += _fileWatcher_Renamed;
        }
        void _fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {

            Debug.WriteLine("資料檔內檔案或目錄被重新命名");
            Debug.WriteLine("發生的事件型別:" + e.ChangeType.ToString());
            Debug.WriteLine("舊檔或舊目錄的完整路徑:" + e.OldFullPath);
            Debug.WriteLine("新檔或新目錄的完整路徑:" + e.FullPath);
            Debug.WriteLine("舊檔或舊目錄名稱:" + e.OldName);
            Debug.WriteLine("新檔或新目錄名稱:" + e.Name);
        }

        void _fileWatcher_Error(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine("監控異常或緩衝溢位:" + e.GetException().StackTrace);
        }

        void _fileWatcher_Disposed(object sender, EventArgs e)
        {
            Debug.WriteLine("FileSystemWatcher Disposed ");
        }

        void _fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("資料檔內發生檔案或目錄被刪除");
            Debug.WriteLine("發生的事件型別:" + e.ChangeType.ToString());
            Debug.WriteLine("被刪除的檔案:" + e.FullPath);
            Debug.WriteLine("檔案名稱:" + e.Name);
        }

        void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("資料檔內發生檔案或目錄被新增");
            Debug.WriteLine("發生的事件型別:" + e.ChangeType.ToString());
            Debug.WriteLine("新增檔案的完整路徑:" + e.FullPath);
            Debug.WriteLine("檔案名稱:" + e.Name);
        }

        void _fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("資料檔內檔案或目錄被更動");
            Debug.WriteLine("發生的事件型別:" + e.ChangeType.ToString());
            Debug.WriteLine("變更的檔案或目錄完整路徑:" + e.FullPath);
            Debug.WriteLine("變更檔案或目錄的名稱:" + e.Name);
        }
        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                this._fileWatcher.EnableRaisingEvents = false;
                
                this._fileWatcher.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[FileSystemListener][Dispose] Error:" + ex.StackTrace);
            }
            finally
            {
                this._fileWatcher = null;
            }
        }
    }
}
