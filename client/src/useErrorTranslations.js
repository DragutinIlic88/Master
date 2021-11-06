import { useTranslation } from "react-i18next";

export const useErrorTranslations = () => {
  const { t } = useTranslation();
  const translateErrors = (errors) => {
    if (typeof errors === "string") {
      const errorLabels = errors.split("\n");
      if (errorLabels.length > 1) {
        const errorTranslations = errorLabels.map((l) => t(l)).join("\n");
        return errorTranslations;
      }
      return t(errors);
    }
    return "";
  };

  return { translateErrors };
};
