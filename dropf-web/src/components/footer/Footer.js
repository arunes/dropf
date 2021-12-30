import React, { Component } from "react";

// images
import img_twitter from "/images/icons/twitter.png";
import img_addictiveTips from "/images/referrals/addictive-tips.png";
import img_lifehacker from "/images/referrals/lifehacker.png";
import img_arunes from "/images/arunes-logo.png";
import img_arunes_hover from "/images/arunes-logo-hover.png";

class Footer extends Component {
    constructor() {
        super();
        this.state = { "authorLogo": img_arunes };

        this.authorMouseEnter = this.authorMouseEnter.bind(this);
        this.authorMouseLeave = this.authorMouseLeave.bind(this);
    }

    openModal(e, modal) {
        this.props.openModal(modal);
        e.preventDefault();
    }

    authorMouseEnter() {
        this.setState({ authorLogo: img_arunes_hover });
    }

    authorMouseLeave() {
        this.setState({ authorLogo: img_arunes });
    }

    render() {
        const referrals = [{
            link: "http://lifehacker.com/5882224/how-to-roll-your-own-awesome-file-sharing-service",
            logo: img_lifehacker
        },
        {
            link: "http://www.addictivetips.com/windows-tips/dropf-upload-files-to-multiple-ftp-server-accounts-via-drag-drop/",
            logo: img_addictiveTips
        }];

        const refferalsHtml = referrals.map((r, i) => {
            return <span key={i}>
                <a href={r.link}><img src={r.logo} /></a> {(i < referrals.length - 1) ?
                    <span>&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    : ""}
            </span>
        });

        return <footer>
            <nav>
                <a href="/download/dropf-setup.exe">download</a>
                <span>.</span>
                <a href="#" onClick={(e) => { this.openModal(e, "versionHistory"); }}>version history</a>
                <span>.</span>
                <a href="#" onClick={(e) => { this.openModal(e, "screenShots") }}>screen shots</a>
                <span>.</span>
                <a href="#" onClick={(e) => { this.openModal(e, "about") }}>about</a>
                <span>.</span>
                <a href="#" onClick={(e) => { this.openModal(e, "license") }}>license</a>
                <span>.</span>
                <a href="http://twitter.com/dropf" target="_blank"><img src={img_twitter} alt="twitter" /> follow on twitter</a> .
            </nav>

            <div className="referrals">
                {refferalsHtml}
            </div>

            <div className="beta">* This software is still in development, there is no stable version at this time. All versions should consider as beta until its declared as stable.</div>

            <div className="version">
                Current Version: 0.2.8 . Last Update: 4/11/2019 . downloaded 50K+ times
            </div>

            <div className="author">
                <a href="http://arunes.com" target="_blank"><img src={this.state.authorLogo} onMouseEnter={this.authorMouseEnter} onMouseLeave={this.authorMouseLeave} alt="arunes" /></a>
            </div>
        </footer>
    }
}

export default Footer;