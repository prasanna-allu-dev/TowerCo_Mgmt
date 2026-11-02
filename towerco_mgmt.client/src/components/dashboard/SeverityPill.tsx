import { SEVERITY_LABELS } from '@/constants/domain';
import type { Severity } from '@/types';

function SeverityPill({ severity, label }: { severity: Severity; label?: string }) {
    return <span className={`pill tone-${severity}`}>{label ?? SEVERITY_LABELS[severity]}</span>;
}

export default SeverityPill;
