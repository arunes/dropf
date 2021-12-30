import React from "react";
import ReactDOM from "react-dom";
import WebFont from "webfontloader";
import ReactGA from "react-ga";
import App from "./components/App"

// import styles
import "./css/site.scss"

// initialize web fonts
WebFont.load({
    google: {
        families: ["Pontano Sans"]
    }
});

// initialize google analytics
ReactGA.initialize("G-75JCZ1LMH9");
ReactGA.pageview(window.location.pathname + window.location.search);

ReactDOM.render(<App />, document.getElementById("app"));