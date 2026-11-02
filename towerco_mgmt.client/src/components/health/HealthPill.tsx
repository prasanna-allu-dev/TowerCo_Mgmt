import { SeverityPill } from '@/components/dashboard';
import { HEALTH_SEVERITY } from '@/constants/health';
import type { HealthStatus } from '@/types';

function HealthPill({ status }: { status: HealthStatus }) {
    return <SeverityPill severity={HEALTH_SEVERITY[status]} label={status} />;
}

export default HealthPill;
