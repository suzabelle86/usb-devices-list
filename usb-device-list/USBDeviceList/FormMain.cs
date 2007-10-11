using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace USBDeviceList
{
    public partial class FormMain : Form
    {
        private string mFolder;
        private Thread mWorkingThread;
        private Process mProcess;

        private CUSBDevices mDevice;

        public FormMain ()
        {
            InitializeComponent();
            mFolder = string.Empty;
            mWorkingThread = new Thread( new ThreadStart( StartUSBDeviceEnum ) );
            mProcess = new Process();
        }

        private void btnExit_Click ( object sender, EventArgs e )
        {
            this.Close();
        }

        private void btnBrowse_Click ( object sender, EventArgs e )
        {
            DialogResult dlg = fldBrowse.ShowDialog( this );

            if ( !dlg.Equals( DialogResult.Cancel ) )
            {
                txtPath.Text = mFolder = fldBrowse.SelectedPath;
                btnStart.Enabled = true;
            }
            else
            {
                if ( string.IsNullOrEmpty( mFolder ) )
                    btnStart.Enabled = false;
            }
        }

        private void btnStart_Click ( object sender, EventArgs e )
        {
            mProcess.StartInfo = new ProcessStartInfo( mFolder );
            
            txtPath.Text = string.Empty;
            btnStart.Enabled = false;
            btnExit.Enabled = false;

            mWorkingThread.Start();
        }

        private void StartUSBDeviceEnum ()
        {
            mDevice = new CUSBDevices();
            mDevice.StartProgress += new EventHandler( mDevice_StartProgress );
            mDevice.EndProgress += new EventHandler( mDevice_EndProgress );


            mDevice.Start( mFolder );
        }

        private void mDevice_StartProgress ( object sender, EventArgs e )
        {
            if ( btnStart.InvokeRequired )
                this.Invoke( new EventHandler( mDevice_StartProgress ), new object [] { sender, e } );
            else
            {
                btnStart.Enabled = false;
                btnExit.Enabled = false;
            }
        }


        private void mDevice_EndProgress ( object sender, EventArgs e )
        {
            if ( btnExit.InvokeRequired )
                this.Invoke( new EventHandler( mDevice_EndProgress ), new object [] { sender, e } );
            else
            {
                btnExit.Enabled = true;
                mFolder = string.Empty;
                mProcess.Start();
            }
        }
    }//class close
}//namespace close