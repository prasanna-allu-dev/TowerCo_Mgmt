import { SEVERITY_LABELS } from '@/shared/constants/domain';
import type { Severity } from '@/shared/types/domain';

function SeverityPill({ severity, label }: { severity: Severity; label?: string }) {
    return <span className={`pill tone-${severity}`}>{label ?? SEVERITY_LABELS[severity]}</span>;
}

export default SeverityPill;
