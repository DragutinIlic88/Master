import React from "react";

import Language from "./Language/Language";
import classes from "./LanguageList.module.css";

/**
 * Component creates and displays list of Language components passed to it
 * via props object. Proper styleing is applied.
 * @param {object} props
 */
const languageList = (props) => {
  const capitalizeFirstLetter = (string) => {
    return string.charAt(0).toUpperCase() + string.slice(1);
  };

  const languages = props.languages.map((lang) => {
    const capLang = capitalizeFirstLetter(lang);
    return (
      <Language
        change={() => {
          props.changeLanguage(lang);
        }}
        language={capLang}
        key={lang}
      />
    );
  });

  const divStyle = {
    width: 140 + languages.length * 80 + "px",
  };

  return (
    <div style={divStyle} className={classes.LanguageList}>
      <span className={classes.ChangeLanguage}>{props.labelText}</span>
      {languages}
    </div>
  );
};

export default languageList;
