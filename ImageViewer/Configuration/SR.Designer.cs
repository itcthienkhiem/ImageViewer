﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18063
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Configuration {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClearCanvas.ImageViewer.Configuration.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 {0} other servers exist with the same AE Title, but different host names and/or ports. Are you sure you want to apply this change? 的本地化字符串。
        /// </summary>
        internal static string ConfirmAETitleConflict_MultipleServers {
            get {
                return ResourceManager.GetString("ConfirmAETitleConflict_MultipleServers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Another server exists with the same AE Title, but a different host name and/or port. Are you sure you want to apply this change? 的本地化字符串。
        /// </summary>
        internal static string ConfirmAETitleConflict_OneServer {
            get {
                return ResourceManager.GetString("ConfirmAETitleConflict_OneServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 If you wish to keep your existing studies, you must manually copy them to the new
        ///location before restarting the local service. Otherwise, you should restart the local service
        ///and immediately re-index. 的本地化字符串。
        /// </summary>
        internal static string DescriptionFileStoreChanged {
            get {
                return ResourceManager.GetString("DescriptionFileStoreChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Please check the servers that should be automatically searched for prior studies. 的本地化字符串。
        /// </summary>
        internal static string DescriptionPriorsServers {
            get {
                return ResourceManager.GetString("DescriptionPriorsServers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The file store location cannot be changed unless the local service is stopped. Once the file store has changed, you must either manually copy files over from the old location before restarting the local service, or simply restart the local service and immediately re-index.
        ///
        ///If the maximum disk usage is exceeded, the Workstation will refuse to import images or receive them over the network in order to avoid exhausting the disk. If the used space is reaching the maximum, it is recommended you increase the m [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string DescriptionStorageOptions {
            get {
                return ResourceManager.GetString("DescriptionStorageOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 DICOM Send 的本地化字符串。
        /// </summary>
        internal static string DicomSendConfiguration {
            get {
                return ResourceManager.GetString("DicomSendConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Example Group 的本地化字符串。
        /// </summary>
        internal static string ExampleGroup {
            get {
                return ResourceManager.GetString("ExampleGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Rm 101 的本地化字符串。
        /// </summary>
        internal static string ExampleLocation {
            get {
                return ResourceManager.GetString("ExampleLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Example Server 的本地化字符串。
        /// </summary>
        internal static string ExampleServer {
            get {
                return ResourceManager.GetString("ExampleServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} (Offline)
        ///The configuration is currently unavailable. 的本地化字符串。
        /// </summary>
        internal static string FormatLocalServerConfigurationUnavailable {
            get {
                return ResourceManager.GetString("FormatLocalServerConfigurationUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0}
        ///AE Title: {1}
        ///Host: {2}
        ///Listening Port: {3} 的本地化字符串。
        /// </summary>
        internal static string FormatLocalServerDetails {
            get {
                return ResourceManager.GetString("FormatLocalServerDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} (Offline)
        ///AE Title: {1}
        ///Host: {2}
        ///Listening Port: {3} 的本地化字符串。
        /// </summary>
        internal static string FormatLocalServerOfflineDetails {
            get {
                return ResourceManager.GetString("FormatLocalServerOfflineDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The server properties conflict with an existing entry ({0}).
        ///Each server must have a unique Name and cannot have the same AE Title, Host and Port as another server. 的本地化字符串。
        /// </summary>
        internal static string FormatServerConflict {
            get {
                return ResourceManager.GetString("FormatServerConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0}
        ///AE Title: {1}
        ///Host: {2}
        ///Listening Port: {3} 的本地化字符串。
        /// </summary>
        internal static string FormatServerDetails {
            get {
                return ResourceManager.GetString("FormatServerDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Server Group &apos;{0}&apos; conflicts with &apos;{1}&apos;.
        ///Please choose another Server Group Name. 的本地化字符串。
        /// </summary>
        internal static string FormatServerGroupConflict {
            get {
                return ResourceManager.GetString("FormatServerGroupConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Streaming Header Port: {0}
        ///Streaming Image Port: {1} 的本地化字符串。
        /// </summary>
        internal static string FormatStreamingDetails {
            get {
                return ResourceManager.GetString("FormatStreamingDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 右键弹出菜单 的本地化字符串。
        /// </summary>
        internal static string LabelContextMenu {
            get {
                return ResourceManager.GetString("LabelContextMenu", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 主菜单 的本地化字符串。
        /// </summary>
        internal static string LabelMainMenu {
            get {
                return ResourceManager.GetString("LabelMainMenu", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 工具栏 的本地化字符串。
        /// </summary>
        internal static string LabelToolbar {
            get {
                return ResourceManager.GetString("LabelToolbar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Start Local Service 的本地化字符串。
        /// </summary>
        internal static string LinkLabelStartLocalService {
            get {
                return ResourceManager.GetString("LinkLabelStartLocalService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Stop Local Service 的本地化字符串。
        /// </summary>
        internal static string LinkLabelStopLocalService {
            get {
                return ResourceManager.GetString("LinkLabelStopLocalService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Local 的本地化字符串。
        /// </summary>
        internal static string LocalConfiguration {
            get {
                return ResourceManager.GetString("LocalConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Location: {0} 的本地化字符串。
        /// </summary>
        internal static string Location {
            get {
                return ResourceManager.GetString("Location", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add server 的本地化字符串。
        /// </summary>
        internal static string MenuAddServer {
            get {
                return ResourceManager.GetString("MenuAddServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add server group 的本地化字符串。
        /// </summary>
        internal static string MenuAddServerGroup {
            get {
                return ResourceManager.GetString("MenuAddServerGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &amp;菜单定制 的本地化字符串。
        /// </summary>
        internal static string MenuCustomizeActionModels {
            get {
                return ResourceManager.GetString("MenuCustomizeActionModels", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 删除 的本地化字符串。
        /// </summary>
        internal static string MenuDelete {
            get {
                return ResourceManager.GetString("MenuDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 编辑 的本地化字符串。
        /// </summary>
        internal static string MenuEdit {
            get {
                return ResourceManager.GetString("MenuEdit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 系统参数配置 的本地化字符串。
        /// </summary>
        internal static string MenuSharedConfiguration {
            get {
                return ResourceManager.GetString("MenuSharedConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Verify 的本地化字符串。
        /// </summary>
        internal static string MenuVerify {
            get {
                return ResourceManager.GetString("MenuVerify", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Failed to update action models 的本地化字符串。
        /// </summary>
        internal static string MessageActionModelUpdateFailure {
            get {
                return ResourceManager.GetString("MessageActionModelUpdateFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 File Store and Study Deletion must be changed separately 的本地化字符串。
        /// </summary>
        internal static string MessageCannotChangeFileStoreAndStudyDeletionSimultaneously {
            get {
                return ResourceManager.GetString("MessageCannotChangeFileStoreAndStudyDeletionSimultaneously", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 C-ECHO Verification: 的本地化字符串。
        /// </summary>
        internal static string MessageCEchoVerificationPrefix {
            get {
                return ResourceManager.GetString("MessageCEchoVerificationPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似     {0}: fail     的本地化字符串。
        /// </summary>
        internal static string MessageCEchoVerificationSingleServerResultFail {
            get {
                return ResourceManager.GetString("MessageCEchoVerificationSingleServerResultFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似     {0}: successful     的本地化字符串。
        /// </summary>
        internal static string MessageCEchoVerificationSingleServerResultSuccess {
            get {
                return ResourceManager.GetString("MessageCEchoVerificationSingleServerResultSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Are you sure you want to delete the selected server? 的本地化字符串。
        /// </summary>
        internal static string MessageConfirmDeleteServer {
            get {
                return ResourceManager.GetString("MessageConfirmDeleteServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Are you sure you want to delete the selected server group? 的本地化字符串。
        /// </summary>
        internal static string MessageConfirmDeleteServerGroup {
            get {
                return ResourceManager.GetString("MessageConfirmDeleteServerGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Failed to save priors server changes. 的本地化字符串。
        /// </summary>
        internal static string MessageFailedToSavePriorsServers {
            get {
                return ResourceManager.GetString("MessageFailedToSavePriorsServers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 File store has changed 的本地化字符串。
        /// </summary>
        internal static string MessageFileStoreChanged {
            get {
                return ResourceManager.GetString("MessageFileStoreChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Host Name cannot be empty. 的本地化字符串。
        /// </summary>
        internal static string MessageHostNameCannotBeEmpty {
            get {
                return ResourceManager.GetString("MessageHostNameCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The shortcut {0} is already assigned to {1}. Do you wish to reassign the shortcut? 的本地化字符串。
        /// </summary>
        internal static string MessageKeyStrokeAlreadyAssigned {
            get {
                return ResourceManager.GetString("MessageKeyStrokeAlreadyAssigned", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The shortcut {0} is reserved and cannot be reassigned. 的本地化字符串。
        /// </summary>
        internal static string MessageKeyStrokeReserved {
            get {
                return ResourceManager.GetString("MessageKeyStrokeReserved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The global tool for {0} is already assigned to {1}. You will not be able to use the global shortcut for {1} whenever this tool is active. Do you still wish to assign this mouse button? 的本地化字符串。
        /// </summary>
        internal static string MessageMouseButtonActiveToolAssignmentConflict {
            get {
                return ResourceManager.GetString("MessageMouseButtonActiveToolAssignmentConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The global tool for {0} is already assigned to {1}. Do you wish to reassign the mouse button shortcut? 的本地化字符串。
        /// </summary>
        internal static string MessageMouseButtonGlobalToolAlreadyAssigned {
            get {
                return ResourceManager.GetString("MessageMouseButtonGlobalToolAlreadyAssigned", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 There are active tools for {0} (e.g. {1}). You will not be able to use this global shortcut whenever {1} is active. Do you still wish to assign this mouse button? 的本地化字符串。
        /// </summary>
        internal static string MessageMouseButtonGlobalToolAssignmentConflict {
            get {
                return ResourceManager.GetString("MessageMouseButtonGlobalToolAssignmentConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The default tool for {0} is already assigned to {1}. Do you wish to reassign the mouse button? 的本地化字符串。
        /// </summary>
        internal static string MessageMouseButtonInitialToolAlreadyAssigned {
            get {
                return ResourceManager.GetString("MessageMouseButtonInitialToolAlreadyAssigned", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No servers have been selected.  Please select at least one server and try again. 的本地化字符串。
        /// </summary>
        internal static string MessageNoServersSelected {
            get {
                return ResourceManager.GetString("MessageNoServersSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Port must be between 1 and 65535. 的本地化字符串。
        /// </summary>
        internal static string MessagePortInvalid {
            get {
                return ResourceManager.GetString("MessagePortInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The AE Title contains invalid characters. 的本地化字符串。
        /// </summary>
        internal static string MessageServerAEInvalidCharacters {
            get {
                return ResourceManager.GetString("MessageServerAEInvalidCharacters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The AE Title must be between 1 and 16 characters. 的本地化字符串。
        /// </summary>
        internal static string MessageServerAEInvalidLength {
            get {
                return ResourceManager.GetString("MessageServerAEInvalidLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Server Group Name cannot be empty. 的本地化字符串。
        /// </summary>
        internal static string MessageServerGroupNameCannotBeEmpty {
            get {
                return ResourceManager.GetString("MessageServerGroupNameCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Server Name cannot be empty. 的本地化字符串。
        /// </summary>
        internal static string MessageServerNameCannotBeEmpty {
            get {
                return ResourceManager.GetString("MessageServerNameCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Click for an explanation of study storage options. 的本地化字符串。
        /// </summary>
        internal static string MessageStorageHelp {
            get {
                return ResourceManager.GetString("MessageStorageHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 You do not have permission to view System Configuration. 的本地化字符串。
        /// </summary>
        internal static string MessageSystemConfigurationNoPermission {
            get {
                return ResourceManager.GetString("MessageSystemConfigurationNoPermission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Failed to start the local service. See the logs for more details. 的本地化字符串。
        /// </summary>
        internal static string MessageUnableToStartLocalService {
            get {
                return ResourceManager.GetString("MessageUnableToStartLocalService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Failed to stop the local service. See the logs for more details. 的本地化字符串。
        /// </summary>
        internal static string MessageUnableToStopLocalService {
            get {
                return ResourceManager.GetString("MessageUnableToStopLocalService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 窗口配置 的本地化字符串。
        /// </summary>
        internal static string MonitorConfiguration {
            get {
                return ResourceManager.GetString("MonitorConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 My Servers 的本地化字符串。
        /// </summary>
        internal static string MyServersTitle {
            get {
                return ResourceManager.GetString("MyServersTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 My Studies 的本地化字符串。
        /// </summary>
        internal static string MyStudiesTitle {
            get {
                return ResourceManager.GetString("MyStudiesTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 N/A 的本地化字符串。
        /// </summary>
        internal static string NotApplicable {
            get {
                return ResourceManager.GetString("NotApplicable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Context Menu (Image) 的本地化字符串。
        /// </summary>
        internal static string PathContextMenuConfiguration {
            get {
                return ResourceManager.GetString("PathContextMenuConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Priors Servers 的本地化字符串。
        /// </summary>
        internal static string PriorsServersConfiguration {
            get {
                return ResourceManager.GetString("PriorsServersConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Publishing 的本地化字符串。
        /// </summary>
        internal static string PublishingConfiguration {
            get {
                return ResourceManager.GetString("PublishingConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 DICOM Server 的本地化字符串。
        /// </summary>
        internal static string ServerConfiguration {
            get {
                return ResourceManager.GetString("ServerConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Study Storage 的本地化字符串。
        /// </summary>
        internal static string StorageConfiguration {
            get {
                return ResourceManager.GetString("StorageConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add New Server 的本地化字符串。
        /// </summary>
        internal static string TitleAddNewServer {
            get {
                return ResourceManager.GetString("TitleAddNewServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add New Server Group 的本地化字符串。
        /// </summary>
        internal static string TitleAddServerGroup {
            get {
                return ResourceManager.GetString("TitleAddServerGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Customize Menus and Toolbars 的本地化字符串。
        /// </summary>
        internal static string TitleCustomizeActionModels {
            get {
                return ResourceManager.GetString("TitleCustomizeActionModels", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Edit Server 的本地化字符串。
        /// </summary>
        internal static string TitleEditServer {
            get {
                return ResourceManager.GetString("TitleEditServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Edit Server Group 的本地化字符串。
        /// </summary>
        internal static string TitleEditServerGroup {
            get {
                return ResourceManager.GetString("TitleEditServerGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 My Servers 的本地化字符串。
        /// </summary>
        internal static string TitleMyServers {
            get {
                return ResourceManager.GetString("TitleMyServers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Select File Store Folder 的本地化字符串。
        /// </summary>
        internal static string TitleSelectFileStore {
            get {
                return ResourceManager.GetString("TitleSelectFileStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 System Configuration 的本地化字符串。
        /// </summary>
        internal static string TitleSharedConfiguration {
            get {
                return ResourceManager.GetString("TitleSharedConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add server 的本地化字符串。
        /// </summary>
        internal static string ToolbarAddServer {
            get {
                return ResourceManager.GetString("ToolbarAddServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add server group 的本地化字符串。
        /// </summary>
        internal static string ToolbarAddServerGroup {
            get {
                return ResourceManager.GetString("ToolbarAddServerGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Delete 的本地化字符串。
        /// </summary>
        internal static string ToolbarDelete {
            get {
                return ResourceManager.GetString("ToolbarDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Edit 的本地化字符串。
        /// </summary>
        internal static string ToolbarEdit {
            get {
                return ResourceManager.GetString("ToolbarEdit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Verify 的本地化字符串。
        /// </summary>
        internal static string ToolbarVerify {
            get {
                return ResourceManager.GetString("ToolbarVerify", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add server 的本地化字符串。
        /// </summary>
        internal static string TooltipAddServer {
            get {
                return ResourceManager.GetString("TooltipAddServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Add server group 的本地化字符串。
        /// </summary>
        internal static string TooltipAddServerGroup {
            get {
                return ResourceManager.GetString("TooltipAddServerGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Delete server or server group 的本地化字符串。
        /// </summary>
        internal static string TooltipDelete {
            get {
                return ResourceManager.GetString("TooltipDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Edit server or server group 的本地化字符串。
        /// </summary>
        internal static string TooltipEdit {
            get {
                return ResourceManager.GetString("TooltipEdit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Verify server or server group 的本地化字符串。
        /// </summary>
        internal static string TooltipVerify {
            get {
                return ResourceManager.GetString("TooltipVerify", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The AE Title contains invalid characters. 的本地化字符串。
        /// </summary>
        internal static string ValidationAETitleInvalidCharacters {
            get {
                return ResourceManager.GetString("ValidationAETitleInvalidCharacters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The AE Title must be between 1 and 16 characters. 的本地化字符串。
        /// </summary>
        internal static string ValidationAETitleLengthIncorrect {
            get {
                return ResourceManager.GetString("ValidationAETitleLengthIncorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The local service must be started before the automatic study deletion behaviour can be changed.
        ///You can click the &quot;Start Local Service&quot; link now, or cancel your changes. 的本地化字符串。
        /// </summary>
        internal static string ValidationCannotChangeDeletionRule {
            get {
                return ResourceManager.GetString("ValidationCannotChangeDeletionRule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The specified drive ({0}) does not exist. 的本地化字符串。
        /// </summary>
        internal static string ValidationDriveDoesNotExist {
            get {
                return ResourceManager.GetString("ValidationDriveDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The specified path is invalid. 的本地化字符串。
        /// </summary>
        internal static string ValidationDriveInvalid {
            get {
                return ResourceManager.GetString("ValidationDriveInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The local service must be stopped before the file store can be changed.
        ///You can click the &quot;Stop Local Service&quot; link now, or cancel your changes. 的本地化字符串。
        /// </summary>
        internal static string ValidationMessageCannotChangeFileStore {
            get {
                return ResourceManager.GetString("ValidationMessageCannotChangeFileStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The Port must be between 1 and 65535. 的本地化字符串。
        /// </summary>
        internal static string ValidationPortOutOfRange {
            get {
                return ResourceManager.GetString("ValidationPortOutOfRange", resourceCulture);
            }
        }
    }
}
