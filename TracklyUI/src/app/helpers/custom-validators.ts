import {
  AbstractControl,
  FormControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';

export function createPasswordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value: string = control.value;
    if (!value || value.length < 8) {
      return { passwordStrength: true };
    }
    const hasUpperCase = /[A-Z]+/.test(value);
    const hasLowerCase = /[a-z]+/.test(value);
    const hasNumeric = /[0-9]+/.test(value);
    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric;
    return !passwordValid ? { passwordStrength: true } : null;
  };
}

export function createConfirmPasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const confirmPass = control.value;
    const pass = control.parent?.get('password')?.value;
    return pass === confirmPass ? null : { confirmPassword: true };
  };
}

export function createUrlValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const url = control.value;
    try {
      new URL(url);
      return null;
    } catch (_) {
      return { urlPath: true };
    }
  };
}

export function createUrlPathValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const urlPath = control.value;
    try {
      new URL(`http://localhost/` + urlPath);
      return null;
    } catch (_) {
      return { urlPath: true };
    }
  };
}

export function getErrorMessage(control: FormControl): string | void {
  const classField = control;
  if (classField.hasError('required')) {
    return 'Field is required';
  } else if (classField.hasError('minlength')) {
    const error = classField.getError('minlength');
    return `Field requires at least ${error.requiredLength} symbols`;
  } else if (classField.hasError('maxlength')) {
    const error = classField.getError('maxlength');
    return `Field requires not more than ${error.requiredLength} symbols`;
  } else if (classField.hasError('email')) {
    return 'Incorrect email address format';
  } else if (classField.hasError('passwordStrength')) {
    return 'Password must contain 8-128 symbols ([a-z], [A-Z], [0-9])';
  } else if (classField.hasError('confirmPassword')) {
    return "Passwords don't match";
  } else if (classField.hasError('url')) {
    return 'Invalid Url format';
  } else if (classField.hasError('urlPath')) {
    return 'Invalid Url path format';
  }
}
