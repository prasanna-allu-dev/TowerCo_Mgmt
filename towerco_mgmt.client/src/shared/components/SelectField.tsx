export interface SelectOption {
    value: string;
    label: string;
}

interface SelectFieldProps {
    label: string;
    value: string;
    options: readonly (string | SelectOption)[];
    onChange: (value: string) => void;
    placeholder?: string;
    required?: boolean;
    className?: string;
}

const toOption = (o: string | SelectOption): SelectOption => (typeof o === 'string' ? { value: o, label: o } : o);

function SelectField({ label, value, options, onChange, placeholder, required, className }: SelectFieldProps) {
    return (
        <label className={className}>{label}
            <select className="input" required={required} value={value} onChange={e => onChange(e.target.value)}>
                {placeholder !== undefined && <option value="">{placeholder}</option>}
                {options.map(toOption).map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
            </select>
        </label>
    );
}

export default SelectField;
