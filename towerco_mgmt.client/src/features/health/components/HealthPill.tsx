import { SeverityPill } from '@/shared/components';
import { HEALTH_SEVERITY } from '@/features/health/constants/health';
import type { HealthStatus } from '@/features/health/types';

function HealthPill({ status }: { status: HealthStatus }) {
    return <SeverityPill severity={HEALTH_SEVERITY[status]} label={status} />;
}

export default HealthPill;
