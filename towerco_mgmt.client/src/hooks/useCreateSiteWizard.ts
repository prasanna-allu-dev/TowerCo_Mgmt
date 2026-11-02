import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { sitesApi } from '@/api';
import { API_ERROR_FALLBACK } from '@/constants/api';
import { EMPTY_FORM, LAST_STEP, STEP_VALIDATORS, toRequest } from '@/components/configuration';
import type { FormErrors, FormField, FormValues, WizardStep } from '@/components/configuration';

/** State machine for the create tower site wizard: validate per step, submit on the last step. */
export function useCreateSiteWizard() {
    const navigate = useNavigate();
    const [values, setValues] = useState<FormValues>(EMPTY_FORM);
    const [errors, setErrors] = useState<FormErrors>({});
    const [step, setStep] = useState<WizardStep>(0);
    const [submitting, setSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState<string>();

    const onChange = (field: FormField, value: string) => {
        setValues(v => ({ ...v, [field]: value }));
        setErrors(e => ({ ...e, [field]: undefined }));
    };

    const validate = (): boolean => {
        const found = STEP_VALIDATORS[step](values);
        setErrors(found);
        return Object.keys(found).length === 0;
    };

    const next = () => validate() && setStep(s => Math.min(s + 1, LAST_STEP) as WizardStep);
    const back = () => setStep(s => Math.max(s - 1, 0) as WizardStep);

    const submit = async () => {
        if (!validate()) return;
        setSubmitting(true);
        setSubmitError(undefined);
        try {
            const created = await sitesApi.create(toRequest(values));
            navigate(`/sites/${created.id}`);
        } catch (e) {
            setSubmitError(e instanceof Error ? e.message : API_ERROR_FALLBACK);
        } finally {
            setSubmitting(false);
        }
    };

    return { values, errors, step, submitting, submitError, onChange, next, back, submit };
}
