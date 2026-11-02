import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { sitesApi } from '@/features/sites/api';
import { EMPTY_FORM, LAST_STEP, STEP_VALIDATORS, toRequest } from '@/features/configuration/components';
import type { FormErrors, FormField, FormValues, WizardStep } from '@/features/configuration/components';
import { sitePath } from '@/shared/constants/routes';
import { useAsyncAction } from '@/shared/hooks/useAsyncAction';

/** Field values and per-step validation for the wizard. */
function useStepForm(step: WizardStep) {
    const [values, setValues] = useState<FormValues>(EMPTY_FORM);
    const [errors, setErrors] = useState<FormErrors>({});

    const onChange = (field: FormField, value: string) => {
        setValues(v => ({ ...v, [field]: value }));
        setErrors(e => ({ ...e, [field]: undefined }));
    };
    const validate = (): boolean => {
        const found = STEP_VALIDATORS[step](values);
        setErrors(found);
        return Object.keys(found).length === 0;
    };
    return { values, errors, onChange, validate };
}

/** State machine for the create tower site wizard: validate per step, submit on the last step. */
export function useCreateSiteWizard() {
    const navigate = useNavigate();
    const [step, setStep] = useState<WizardStep>(0);
    const form = useStepForm(step);
    const save = useAsyncAction(() => sitesApi.create(toRequest(form.values)), created => navigate(sitePath(created.id)));

    const next = () => form.validate() && setStep(s => Math.min(s + 1, LAST_STEP) as WizardStep);
    const back = () => setStep(s => Math.max(s - 1, 0) as WizardStep);
    const submit = () => form.validate() && save.run();

    const { values, errors, onChange } = form;
    return { values, errors, step, submitting: save.pending, submitError: save.error, onChange, next, back, submit };
}
