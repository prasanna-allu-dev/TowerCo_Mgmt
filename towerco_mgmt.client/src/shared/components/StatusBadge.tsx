import type { SiteStatus } from '@/shared/types/domain';

function StatusBadge({ status }: { status: SiteStatus }) {
    return <span className={`badge badge-${status.toLowerCase()}`}>{status}</span>;
}

export default StatusBadge;
