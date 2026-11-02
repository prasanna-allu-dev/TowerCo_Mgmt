import type { ReactNode } from 'react';

interface FormFieldProps {
    label: string;
    error?: string;
    hint?: string;
    children: ReactNode;
}

function FormField({ label, error, hint, children }: FormFieldProps) {
    return (
        <label className={`field${error ? ' field-invalid' : ''}`}>
            <span className="field-label">{label}</span>
            {children}
            {error ? <span className="field-error">{error}</span> : hint && <span className="field-hint">{hint}</span>}
        </label>
    );
}

export default FormField;
