import React, { Component } from "react";
import Modal from "react-responsive-modal";
import Header from "./header/Header";
import Home from "./home/Home";
import Footer from "./footer/Footer";

// modals
import VersionHistory from "./version-history/VersionHistory";
import ScreenShots from "./screen-shots/ScreenShots";
import About from "./about/About";
import License from "./license/License";

class App extends Component {
    constructor() {
        super();
        this.state = { modalIsOpen: false };
        this.openModal = this.openModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
    }

    closeModal() {
        this.setState({ modalIsOpen: false });
    }

    openModal(modal) {
        this.setState({ modalIsOpen: true, activeModal: modal });
    }

    render() {
        var modalHtml;
        switch (this.state.activeModal) {
            case "versionHistory":
                modalHtml = <VersionHistory />;
                break;

            case "screenShots":
                modalHtml = <ScreenShots />;
                break;

            case "about":
                modalHtml = <About />;
                break;

            case "license":
                modalHtml = <License />;
                break;

        }

        return <div>
            <Header></Header>
            <Home></Home>
            <Footer openModal={this.openModal}></Footer>

            <Modal open={this.state.modalIsOpen} onClose={this.closeModal} center>
                {modalHtml}
            </Modal>
        </div>
    }
}

export default App;