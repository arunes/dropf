import React, { Component } from "react";

// import images
import ss_main_idle from '/images/screen-shots/01-main-idle.jpg'
import ss_main_hover from '/images/screen-shots/02-main-hover.jpg'
import ss_main_uploading from '/images/screen-shots/03-main-uploading.jpg'
import ss_completed from '/images/screen-shots/04-completed.jpg'
import ss_context_menu from '/images/screen-shots/05-context-menu.jpg'
import ss_manage_ftp from '/images/screen-shots/06-manage-ftp.jpg'
import ss_settings from '/images/screen-shots/07-settings.jpg'
import ss_update from '/images/screen-shots/08-update.jpg'

class ScreenShots extends Component {
    render() {
        return <div className="screen-shots">

            <h3>Main window and upload</h3>
            <img src={ss_main_idle} title="Main window" />
            <img src={ss_main_hover} title="Main window" />
            <img src={ss_main_uploading} title="Uploading" />

            <h3>Upload completed</h3>
            <img src={ss_completed} title="Upload completed" />

            <h3>Menu</h3>
            <img src={ss_context_menu} title="Menu" />

            <h3>Manage ftp accounts</h3>
            <img src={ss_manage_ftp} title="Manage ftp accounts" />

            <h3>Settings</h3>
            <img src={ss_settings} title="Settings" />

            <h3>Check for updates</h3>
            <img src={ss_update} title="Check for updates" />

        </div>
    }
}

export default ScreenShots;