import type { SiteStatus } from '@/types';

function StatusBadge({ status }: { status: SiteStatus }) {
    return <span className={`badge badge-${status.toLowerCase()}`}>{status}</span>;
}

export default StatusBadge;
