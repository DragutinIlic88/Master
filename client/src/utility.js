export const emailPattern =
  "^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0–9](?:[a-zA-Z0–9-]{0,61} [a-zA-Z0–9])?(?:.[a-zA-Z0–9](?:[a-zA-Z0–9-]{0,61}[a-zA-Z0–9])?)*$";
export const passwordPattern = "[a-zA-Z0-9!?:,.]{8,}";

export const bankIdPattern = "[0-9]{12}|[0-9]{18}";
export const numberPattern = /^[0-9\b]+$/;

export const mobilePattern = "[+]?[0-9]{9,12}";
export const mobileInsertPattern = /^[0-9+\\-]{0,15}$/;
export const mobileSignsOnly = /^[+]?[0-9]*$/;

export const isValueValid = (value, pattern) => {
  let re = new RegExp(pattern);

  if (re.test(value)) return true;

  return false;
};

export const formElementPatternCheckerDecorator = (
  event,
  onChangeFunction,
  pattern
) => {
  if (pattern) {
    let regex = new RegExp(pattern);
    if (event.target.value === "" || regex.test(event.target.value)) {
      onChangeFunction(event);
    }
  } else {
    onChangeFunction(event);
  }
};

export const extractErrorsFromResponse = (data) => {
  if (data.errors) {
    const error = Object.keys(data.errors)
      .map((key) =>
        data.errors[key].length > 1
          ? data.errors[key].join("\n")
          : data.errors[key][0]
      )
      .join("\n");
    return error;
  }
  return data;
};
