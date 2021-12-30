import React, { Component } from "react";
import { DragDropContainer, DropTarget } from 'react-drag-drop-container';

// import images
import img_logo from "/images/logo.png"
import img_arrow from "/images/arrow.png"

class Header extends Component {
    constructor() {
        super();
        this.state = { dropBoxIcon: "close" };

        this.changeSetupIcon = this.changeSetupIcon.bind(this);
        this.setupDropped = this.setupDropped.bind(this);
    }

    changeSetupIcon(icon) {
        console.log("changeSetupIcon", icon);
        this.setState({ dropBoxIcon: icon });
    }

    setupDropped() {
        this.changeSetupIcon("close");
        window.location.href = "/download/dropf-setup.exe";
    }

    render() {

        return <header>
            <div className="wrapper">
                <div className="slogan">drag, drop and share...</div>
                <div className="arrow"><img src={img_arrow} alt="" /></div>
                <div className="columns">
                    <div className="column left">
                        <DragDropContainer
                            targetKey="setup"
                            onDragStart={()=> this.changeSetupIcon('open')}
                            onDragEnd={()=> this.changeSetupIcon('close')}>
                            <div id="icon-setup"></div>
                        </DragDropContainer>
                    </div>
                    <div className="column center"><img src={img_logo} alt="dropf" /></div>
                    <div id="icon-box-drop" className="column right">
                        <DropTarget
                            targetKey="setup"
                            onDragEnter={()=> this.changeSetupIcon('hover')}
                            onDragLeave={()=> this.changeSetupIcon('open')}
                            onHit={this.setupDropped}>
                            <div id="icon-box" className={this.state.dropBoxIcon}></div>
                        </DropTarget>
                    </div>
                    <br className="clearfix" />
                </div>

                <div className="description">
                    <p className="big"><strong>dropf</strong> is a handy and reliable utility designed to enable you to manage your ftp accounts and to share files with very easy-to-use drag  & drop interface.</p>
                    <p className="small">drag and drop setup icon to dropf box for download <b>dropf</b>..</p>
                </div>
            </div>
        </header >
    }
}

export default Header;