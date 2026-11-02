import { WIZARD_STEPS } from './createSiteForm';
import type { WizardStep } from './createSiteForm';

function stateOf(index: number, current: WizardStep): string {
    if (index < current) return 'done';
    return index === current ? 'current' : 'todo';
}

function WizardSteps({ current }: { current: WizardStep }) {
    return (
        <ol className="wizard-steps">
            {WIZARD_STEPS.map((label, index) => (
                <li key={label} className={`wizard-step ${stateOf(index, current)}`} aria-current={index === current ? 'step' : undefined}>
                    <span className="wizard-dot">{index < current ? '✓' : index + 1}</span>
                    {label}
                </li>
            ))}
        </ol>
    );
}

export default WizardSteps;
