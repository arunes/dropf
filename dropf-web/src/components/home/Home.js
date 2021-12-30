import React, { Component } from "react";

// import images
import de from "/images/flags/de.png"
import es from "/images/flags/es.png"
import ru from "/images/flags/ru.png"
import tr from "/images/flags/tr.png"
import uk from "/images/flags/uk.png"

class Home extends Component {
    render() {
        const specifications = [
            "Folder or multiple file upload",
            "Multiple files can zipped into one file",
            "Define multiple FTP accounts",
            "Customizable theme support",
            "Quick switch between FTP accounts",
            "Upload history",
            "Editable language files",
            "Upload text/file/image from clipboard",
            "Take screenshot and upload with one click",
            "Support 2d1.in url shortener service",
            "Support goo.gl url shortener service",
            "Support is.gd url shortener service",
            "Send files/folders from Windows Send To menu",
            "Can encrypt zip files"
        ];

        const langugages = [{
            flag: tr,
            lang: "Turkish"
        },{
            flag: uk,
            lang: "English"
        },{
            flag: de,
            lang: "German"
        },{
            flag: ru,
            lang: "Russian"
        },{
            flag: es,
            lang: "Spanish"
        }];

        return <div className="content">
            <div className="specs">
                {specifications.map((spec, index) => <span key={index} title={spec}>{spec}</span>)}

                <span>Language Support &nbsp;
                    {langugages.map(lang => <img key={lang.lang} src={lang.flag} alt={lang.lang} title={lang.lang} />)}
                </span>
            </div>
        </div>
    }
}

export default Home;