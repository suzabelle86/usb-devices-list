using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

/*
 * 
 * http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=1564963&SiteID=1
 * 
 * 
 */ 


namespace USBDeviceList
{
    internal delegate void ProgressEventHandler (string Message);
    
    internal class CUSBDevices
    {
        internal event ProgressEventHandler ProgressEvent;
        internal event EventHandler StartProgress;
        internal event EventHandler EndProgress;
        public CUSBDevices ()
        {

        }

        internal void Start ( string Path )
        {
            if ( StartProgress != null )
                StartProgress( this, null );

            string strPath = string.Empty;
//            string strSearch = string.Format( "vid_{0:x4}&pid_{1:x4}", nVid, nPid ); // first, build the path search string
            //Guid gHid;
            //CWin32.HidD_GetHidGuid( out gHid );	// next, get the GUID from Windows that it uses to represent the HID USB interface

            Guid gHid = new Guid( "86e0d1e0808911d09ce408003e301f73" );

            IntPtr hInfoSet = CWin32.SetupDiGetClassDevs( ref gHid, null, IntPtr.Zero, CWin32.DIGCF_DEVICEINTERFACE | CWin32.DIGCF_PRESENT );	// this gets a list of all HID devices currently connected to the computer (InfoSet)
            try
            {
                CWin32.DeviceInterfaceData oInterface = new CWin32.DeviceInterfaceData();	// build up a device interface data block
                oInterface.Size = Marshal.SizeOf( oInterface );
                // Now iterate through the InfoSet memory block assigned within Windows in the call to SetupDiGetClassDevs
                // to get device details for each device connected
                int nIndex = 0;
                while ( CWin32.SetupDiEnumDeviceInterfaces( hInfoSet, 0, ref gHid, (uint)nIndex, ref oInterface ) )	// this gets the device interface information for a device at index 'nIndex' in the memory block
                {
                    string strDevicePath = GetDevicePath( hInfoSet, ref oInterface );	// get the device path (see helper method 'GetDevicePath')
                    //if ( strDevicePath.IndexOf( strSearch ) >= 0 )	// do a string search, if we find the VID/PID string then we found our device!
                    //{
                    //    HIDDevice oNewDevice = (HIDDevice)Activator.CreateInstance( oType );	// create an instance of the class for this device
                    //    oNewDevice.Initialise( strDevicePath );	// initialise it with the device path
                    //    return oNewDevice;	// and return it
                    //}
                    nIndex++;	// if we get here, we didn't find our device. So move on to the next one.
                }
            }
            finally
            {
                // Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
                CWin32.SetupDiDestroyDeviceInfoList( hInfoSet );
            }
//            return null;	// oops, didn't find our device



            if ( EndProgress != null )
                EndProgress( this, null );
        }

        /// <summary>
        /// Helper method to return the device path given a DeviceInterfaceData structure and an InfoSet handle.
        /// Used in 'FindDevice' so check that method out to see how to get an InfoSet handle and a DeviceInterfaceData.
        /// </summary>
        /// <param name="hInfoSet">Handle to the InfoSet</param>
        /// <param name="oInterface">DeviceInterfaceData structure</param>
        /// <returns>The device path or null if there was some problem</returns>
        private static string GetDevicePath ( IntPtr hInfoSet, ref CWin32.DeviceInterfaceData oInterface )
        {
            uint nRequiredSize = 0;
            // Get the device interface details
            if ( !CWin32.SetupDiGetDeviceInterfaceDetail( hInfoSet, ref oInterface, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero ) )
            {
                CWin32.DeviceInterfaceDetailData oDetail = new CWin32.DeviceInterfaceDetailData();
                oDetail.Size = 5;	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
                if ( CWin32.SetupDiGetDeviceInterfaceDetail( hInfoSet, ref oInterface, ref oDetail, nRequiredSize, ref nRequiredSize, IntPtr.Zero ) )
                {
                    return oDetail.DevicePath;
                }
            }
            return null;
        }

    }//class close
}//namespace close


/*Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Text

Friend Class NativeMethods

#Region " Constants "
    Public Shared ReadOnly GUID_DEVINTERFACE_COMPORT As New Guid("86e0d1e0808911d09ce408003e301f73")
    Public Const CM_REGISTRY_HARDWARE As Integer = 0
    Public Const ERROR_INSUFFICIENT_BUFFER As Integer = 122
    Public Const ERROR_INVALID_DATA As Integer = 13
    Public Const ERROR_NO_MORE_ITEMS As Integer = 259
    Public Const KEY_QUERY_VALUE As Integer = 1
    Public Const RegDisposition_OpenExisting As Integer = 1

#End Region

#Region " Enums "
    Public Enum CRErrorCodes
        CR_SUCCESS = 0
        CR_DEFAULT
        CR_OUT_OF_MEMORY
        CR_INVALID_POINTER
        CR_INVALID_FLAG
        CR_INVALID_DEVNODE
        CR_INVALID_RES_DES
        CR_INVALID_LOG_CONF
        CR_INVALID_ARBITRATOR
        CR_INVALID_NODELIST
        CR_DEVNODE_HAS_REQS
        CR_INVALID_RESOURCEID
        CR_DLVXD_NOT_FOUND             ' WIN 95 ONLY
        CR_NO_SUCH_DEVNODE
        CR_NO_MORE_LOG_CONF
        CR_NO_MORE_RES_DES
        CR_ALREADY_SUCH_DEVNODE
        CR_INVALID_RANGE_LIST
        CR_INVALID_RANGE
        CR_FAILURE
        CR_NO_SUCH_LOGICAL_DEV
        CR_CREATE_BLOCKED
        CR_NOT_SYSTEM_VM            ' WIN 95 ONLY
        CR_REMOVE_VETOED
        CR_APM_VETOED
        CR_INVALID_LOAD_TYPE
        CR_BUFFER_SMALL
        CR_NO_ARBITRATOR
        CR_NO_REGISTRY_HANDLE
        CR_REGISTRY_ERROR
        CR_INVALID_DEVICE_ID
        CR_INVALID_DATA
        CR_INVALID_API
        CR_DEVLOADER_NOT_READY
        CR_NEED_RESTART
        CR_NO_MORE_HW_PROFILES
        CR_DEVICE_NOT_THERE
        CR_NO_SUCH_VALUE
        CR_WRONG_TYPE
        CR_INVALID_PRIORITY
        CR_NOT_DISABLEABLE
        CR_FREE_RESOURCES
        CR_QUERY_VETOED
        CR_CANT_SHARE_IRQ
        CR_NO_DEPENDENT
        CR_SAME_RESOURCES
        CR_NO_SUCH_REGISTRY_KEY
        CR_INVALID_MACHINENAME      ' NT ONLY
        CR_REMOTE_COMM_FAILURE      ' NT ONLY
        CR_MACHINE_UNAVAILABLE      ' NT ONLY
        CR_NO_CM_SERVICES           ' NT ONLY
        CR_ACCESS_DENIED            ' NT ONLY
        CR_CALL_NOT_IMPLEMENTED
        CR_INVALID_PROPERTY
        CR_DEVICE_INTERFACE_ACTIVE
        CR_NO_SUCH_DEVICE_INTERFACE
        CR_INVALID_REFERENCE_STRING
        CR_INVALID_CONFLICT_LIST
        CR_INVALID_INDEX
        CR_INVALID_STRUCTURE_SIZE
        NUM_CR_RESULTS
    End Enum

    Public Enum RegPropertyTypes
        REG_BINARY = 3
        REG_DWORD = 4
        REG_DWORD_BIG_ENDIAN = 5
        REG_DWORD_LITTLE_ENDIAN = 4
        REG_EXPAND_SZ = 2
        REG_MULTI_SZ = 7
        REG_SZ = 1
    End Enum

    <Flags()> _
    Public Enum DeviceFlags As Integer
        DigCFDefault = 1
        DigCFPresent = 2            ' return only devices that are currently present
        DigCFAllClasses = 4         ' gets all classes, ignores the guid...
        DigCFProfile = 8            ' gets only classes that are part of the current hardware profile
        DigCDDeviceInterface = 16   ' Return devices that expose interfaces of the interface class that are specified by ClassGuid.
    End Enum

#End Region

#Region " Structures "
    ' SP_DEVICE_INTERFACE_DATA
    <StructLayout(LayoutKind.Sequential, pack:=1)> _
    Public Structure DeviceInterfaceData
        Public MySize As Integer
        Public InterfaceClassGuid As Guid
        Public Flags As Integer
        Public Reserved As IntPtr
        Public Sub Initialize()
            Me.MySize = Marshal.SizeOf(GetType(DeviceInterfaceData))
        End Sub
    End Structure

    ' SP_DEVINFO_DATA
    <StructLayout(LayoutKind.Sequential, pack:=1)> _
    Public Structure DevinfoData
        Public MySize As Integer
        Public ClassGuid As Guid
        Public DevInst As IntPtr ' DWORD? x64?
        Public Reserved As Integer
        Public Sub Initialize()
            Me.MySize = Marshal.SizeOf(GetType(DevinfoData))
        End Sub
    End Structure

    ' SP_DEVICE_INTERFACE_DETAIL_DATA
    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Public Structure DeviceInterfaceDetailData
        Public MySize As Integer
        Public DevicePath As Short
        Public Sub Initialize()
            Me.MySize = Marshal.SizeOf(GetType(DeviceInterfaceDetailData))
        End Sub
    End Structure
#End Region

#Region " P/Invoke Signatures "

    <DllImport("setupapi.dll")> _
    Public Shared Function CM_Get_Device_ID( _
    ByVal hDeviceInstance As IntPtr, _
    ByVal buffer As System.Text.StringBuilder, _
    ByVal bufferLength As Integer, _
    ByVal mustBeZero As Integer) As CRErrorCodes
    End Function

    <DllImport("cfgmgr32", _
    SetLastError:=True)> _
    Public Shared Function CM_Open_DevNode_Key( _
    ByVal devNode As IntPtr, _
    ByVal samDesired As Integer, _
    ByVal hardwareProfile As Integer, _
    ByVal disposition As Integer, _
    ByRef hKey As IntPtr, _
    ByVal flags As Integer) As CRErrorCodes
    End Function

    <DllImport("setupapi.dll")> _
    Public Shared Function CM_Get_Parent( _
    ByRef hDeviceInstanceParent As IntPtr, _
    ByVal hDeviceInstance As IntPtr, _
    ByVal mustBeZero As Integer) As CRErrorCodes
    End Function

    <System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError:=True)> _
    Public Shared Function RegCloseKey(ByVal hkey As IntPtr) As Integer
    End Function

    <System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError:=True)> _
    Public Shared Function RegQueryValueEx( _
    ByVal hKey As IntPtr, _
    ByVal valueName As String, _
    ByVal reserved As Integer, _
    ByRef type As RegPropertyTypes, _
    ByVal data As System.Text.StringBuilder, _
    ByRef dataSize As Integer) As Integer
    End Function

    <DllImport("setupapi", SetLastError:=True)> _
    Public Shared Function SetupDiDestroyDeviceInfoList( _
    ByVal hDeviceInfoSet As IntPtr) As Boolean
    End Function

    <DllImport("setupapi.dll", SetLastError:=True)> _
    Public Shared Function SetupDiEnumDeviceInterfaces( _
    ByVal deviceInfoSet As IntPtr, _
    ByVal deviceInfoData As IntPtr, _
    <MarshalAs(UnmanagedType.LPStruct)> _
    ByVal interfaceClassGuid As Guid, _
    ByVal memberIndex As Integer, _
    ByRef deviceInterfaceData As DeviceInterfaceData) As Boolean
    End Function

    <DllImport("setupapi", SetLastError:=True)> _
    Public Shared Function SetupDiGetClassDevs( _
    <MarshalAs(UnmanagedType.LPStruct)> _
    ByVal classGuid As System.Guid, _
    ByVal enumerator As String, _
    ByVal hwndParent As IntPtr, _
    ByVal flags As DeviceFlags) As IntPtr
    End Function

    <DllImport("setupapi.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function SetupDiGetDeviceInterfaceDetail( _
    ByVal deviceInfoSet As IntPtr, _
    ByRef deviceInterfaceData As DeviceInterfaceData, _
    ByVal deviceInterfaceDetailData As IntPtr, _
    ByVal deviceInterfaceDetailDataSize As Integer, _
    ByRef requiredSize As Integer, _
    ByRef deviceInfoData As DevinfoData) As Boolean
    End Function

#End Region

End Class

 ------------------------------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Text
Imports WindowsApplication25.NativeMethods ' Rename...

Public Class USBPortEnumerator

    Public Shared Function GetCOMPortNumberForUSBDevice(ByVal vendorID As String, ByVal productID As String) As String
        ' Get a handle to a Device Information set for
        ' all Present Ports on the computer that support
        ' the Device Infomation Interface.
        Dim hDevInfoSet As IntPtr = SetupDiGetClassDevs( _
            GUID_DEVINTERFACE_COMPORT, Nothing, Nothing, _
            DeviceFlags.DigCDDeviceInterface Or DeviceFlags.DigCFPresent)
        If hDevInfoSet.ToInt64 = -1 Then
            Throw New Win32Exception
        End If

        ' We loop though all the Ports.
        Dim portIndex As Integer = 0
        Try
            Do
                ' Get a Device Interface Data structure.
                ' --------------------------------------
                Dim interfaceData As New DeviceInterfaceData
                interfaceData.Initialize()
                Dim result As Boolean = SetupDiEnumDeviceInterfaces( _
                    hDevInfoSet, Nothing, GUID_DEVINTERFACE_COMPORT, _
                    portIndex, interfaceData)
                If result = False Then
                    If Marshal.GetLastWin32Error = ERROR_NO_MORE_ITEMS Then
                        ' We've done all the com ports.
                        Exit Do
                    Else
                        ' Unforseen problem...
                        Throw New Win32Exception
                    End If
                End If

                ' Get a DevInfoDetailData and DeviceInfoData
                ' ------------------------------------------
                Dim infoData As New DevinfoData
                infoData.Initialize()
                Dim requiredSize As Integer
                ' First call to get the required size.
                result = SetupDiGetDeviceInterfaceDetail( _
                    hDevInfoSet, interfaceData, Nothing, _
                    0, requiredSize, infoData)
                ' We expect an insufficient buffer error.
                If Marshal.GetLastWin32Error <> ERROR_INSUFFICIENT_BUFFER Then
                    Throw New Win32Exception
                End If
                ' Create the buffer.
                Dim detailData As New DeviceInterfaceDetailData
                detailData.Initialize()
                Dim devDetailBuffer As IntPtr
                Dim devicePath As String = Nothing
                Try
                    devDetailBuffer = Marshal.AllocHGlobal(requiredSize)
                    Marshal.StructureToPtr(detailData, devDetailBuffer, True)

                    ' Call with the correct buffer
                    result = SetupDiGetDeviceInterfaceDetail(hDevInfoSet, _
                        interfaceData, devDetailBuffer, requiredSize, _
                        requiredSize, infoData)

                    If result = False Then
                        Throw New Win32Exception
                    End If
                Finally
                    Marshal.FreeHGlobal(devDetailBuffer)
                End Try

                ' Is this Port a USB Port? Ask the parent, then it's parent
                ' etc if it is a USB device.
                ' ---------------------------------------------------------------
                Dim startingDevice As IntPtr = infoData.DevInst
                Dim CRResult As CRErrorCodes
                Do
                    Dim hParentDevice As IntPtr = IntPtr.Zero
                    CRResult = CM_Get_Parent(hParentDevice, startingDevice, 0)
                    If CRResult = CRErrorCodes.CR_NO_SUCH_DEVNODE Then
                        ' We hit the top of the pnp tree.
                        Exit Do
                    End If
                    If CRResult <> CRErrorCodes.CR_SUCCESS Then
                        Throw New Exception("Error calling CM_Get_Parent: " & CRResult.ToString)
                    End If
                    Dim sb As New System.Text.StringBuilder(1024)
                    CRResult = CM_Get_Device_ID(hParentDevice, sb, sb.Capacity, 0)
                    If CRResult <> CRErrorCodes.CR_SUCCESS Then
                        Throw New Exception("Error calling CM_Get_Device_ID: " & CRResult.ToString)
                    End If
                    ' We have the pnp string of the parent device.
                    Dim deviceID As String = sb.ToString
                    If deviceID.StartsWith("USB\") Then
                        ' check the version and product ids.
                        Dim vid As String = deviceID.Substring(deviceID.IndexOf("VID_") + 4, 4)
                        Dim pid As String = deviceID.Substring(deviceID.IndexOf("PID_") + 4, 4)
                        If vid.Equals(vendorID) AndAlso pid.Equals(productID) Then
                            Dim hkey As IntPtr = IntPtr.Zero
                            CRResult = CM_Open_DevNode_Key(infoData.DevInst, KEY_QUERY_VALUE, _
                               0, RegDisposition_OpenExisting, hkey, CM_REGISTRY_HARDWARE)
                            If CRResult <> CRErrorCodes.CR_SUCCESS Then
                                Throw New Exception("CM_Open_DevNode_Key error: " & CRResult.ToString)
                            End If
                            Dim comNumber As New StringBuilder(16)
                            Dim type As RegPropertyTypes
                            Dim size As Integer = comNumber.Capacity
                            Dim hresult As Integer = RegQueryValueEx(hkey, "PortName", 0, type, comNumber, size)
                            If hresult <> 0 Then
                                Throw New Win32Exception
                            End If
                            hresult = RegCloseKey(hkey)
                            If hresult <> 0 Then
                                Throw New Win32Exception
                            End If
                            ' After all that...
                            Return comNumber.ToString
                            ' (If you have several ports on one USB device, then
                            ' add this value to some collection and then Exit Do)
                        Else
                            ' it was another usb device.
                            ' no point continuing with this port.
                            Exit Do
                        End If
                    End If
                    ' Do the next parent.
                    startingDevice = hParentDevice
                Loop
                ' Do the next port
                portIndex += 1
            Loop
        Finally
            SetupDiDestroyDeviceInfoList(hDevInfoSet)
        End Try
        ' We did all the ports and didn't find it.
        Return Nothing
    End Function

End Class

 
 */
 