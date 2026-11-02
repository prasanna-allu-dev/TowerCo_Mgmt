import type { ChangeEvent } from 'react';
import type { FormErrors, FormField, FormValues } from './createSiteForm';

export interface StepProps {
    values: FormValues;
    errors: FormErrors;
    onChange: (field: FormField, value: string) => void;
}

type InputEvent = ChangeEvent<HTMLInputElement | HTMLSelectElement>;

/** Binds an input or select to one form field. */
export function bind(props: StepProps, field: FormField) {
    return {
        value: props.values[field],
        onChange: (e: InputEvent) => props.onChange(field, e.target.value),
        className: 'input',
        'aria-invalid': !!props.errors[field],
    };
}
