import { Link } from 'react-router-dom';
import { Card } from '@/shared/components';
import { LAST_STEP, ReviewStep, SiteStep, TowerStep, WIZARD_STEPS, WizardSteps } from '@/features/configuration/components';
import { ROUTES } from '@/shared/constants/routes';
import { useCreateSiteWizard } from '@/features/configuration/hooks/useCreateSiteWizard';
import PageHeader from '@/shared/components/PageHeader';

type Wizard = ReturnType<typeof useCreateSiteWizard>;

function StepBody({ wizard }: { wizard: Wizard }) {
    const { step, values, errors, onChange } = wizard;
    if (step === 0) return <SiteStep values={values} errors={errors} onChange={onChange} />;
    if (step === 1) return <TowerStep values={values} errors={errors} onChange={onChange} />;
    return <ReviewStep values={values} />;
}

function WizardActions({ wizard }: { wizard: Wizard }) {
    const { step, submitting, back, next, submit } = wizard;
    return (
        <div className="form-actions">
            <Link className="btn btn-ghost" to={ROUTES.configuration}>Cancel</Link>
            {step > 0 && <button type="button" className="btn btn-secondary" onClick={back} disabled={submitting}>Back</button>}
            {step < LAST_STEP
                ? <button type="button" className="btn btn-primary" onClick={next}>Next</button>
                : <button type="button" className="btn btn-primary" onClick={submit} disabled={submitting}>{submitting ? 'Creating...' : 'Create site and tower'}</button>}
        </div>
    );
}

function CreateTowerSitePage() {
    const wizard = useCreateSiteWizard();
    return (
        <>
            <Link className="back-link" to={ROUTES.configuration}>← Tower configuration</Link>
            <PageHeader title="New tower site" subtitle="Register a site and its tower in one step (UC1)" />
            <WizardSteps current={wizard.step} />
            <Card title={WIZARD_STEPS[wizard.step]}>
                <StepBody wizard={wizard} />
                {wizard.submitError && <p className="alert-error" role="alert">{wizard.submitError}</p>}
                <WizardActions wizard={wizard} />
            </Card>
        </>
    );
}

export default CreateTowerSitePage;
