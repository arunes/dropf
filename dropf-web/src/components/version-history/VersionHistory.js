import React, { Component } from "react";

class VersionHistory extends Component {
    render() {
        const versionHistory = [
            {
                version: "0.2.8 (02/22/2012)",
                changeLog: [
                    "* fixed: History list not shown properly",
                    "* improved: Timeout increased for big file uploads",
                    "* improved: You can encrypt your zip files now.",
                    "* improved: New icon added to windows send to menu for zip"
                ]
            },
            {
                version: "0.2.7 (02/20/2012)",
                changeLog: [
                    "* fixed: 553 Error on some ftp servers",
                    "* fixed: Automatically zipping files",
                    "* fixed: dropf dissappears",
                    "* improved: Added new reset visual button on dropf context menu"
                ]
            },
            {
                version: "0.2.6 (12/31/2011)",
                changeLog: [
                    "* fixed: Bypassing default proxy settings",
                    "* fixed: You can change ftp port now",
                    "* improved: About screen improved",
                    "* improved: Russian language support",
                    "* fixed: More general tweaks, improvements, fixes and optimizations"
                ]
            },
            {
                version: "0.2.5 (07/14/2011)",
                changeLog: [
                    "* fixed: Url shortener services working properly now",
                    "* fixed: Starting with windows is working properly now",
                    "* improved: Drop with right mouse button now opens ftp sites",
                    "* improved: German language support",
                    "* fixed: More general tweaks, improvements, fixes and optimizations"
                ]
            },
            {
                version: "0.2.4 (07/04/2011)",
                changeLog: [
                    "* improved: Editable language files",
                    "* improved: Upload text/file/image from clipboard",
                    "* improved: Take screenshot and upload with one click",
                    "* improved: Support goo.gl url shortener service",
                    "* improved: Support is.gd url shortener service",
                    "* improved: Send files/folders from Windows Send To menu",
                    "* fixed: More general tweaks, improvements, fixes and optimizations",
                    "* improved: Language support in version check"
                ]
            },
            {
                version: "0.2.0 (06/25/2011)",
                changeLog: [
                    "* improved: Customizable theme support",
                    "* improved: Quick switch between FTP accounts",
                    "* improved: Upload history",
                    "* fixed: More general tweaks, improvements, fixes and optimizations"
                ]
            },
            {
                version: "0.1.0 (06/10/2011)",
                changeLog: [
                    "* improved: Folder or multiple file upload",
                    "* improved: Multiple files zipped into one file automatically",
                    "* improved: Define multiple FTP accounts",
                    "* improved: Support 2d1.in url shortener service",
                    "* fixed: More general tweaks, improvements, fixes and optimizations"
                ]
            }
        ];

        return <div className="version-history">
            {versionHistory.map(v =>
                <ul key={v.version}>
                    <li className="version">{v.version}</li>
                    {v.changeLog.map((c, index) =>
                        <li key={index}>{c}</li>
                    )}
                </ul>
            )}
        </div>
    }
}

export default VersionHistory;