import { useState } from 'react';
import { workOrdersApi } from '@/api';
import { API_ERROR_FALLBACK } from '@/constants/api';
import { ASSIGNEE_REQUIRED_STATUS, FIELD_ENGINEERS, WORK_ORDER_STATUS_LABELS, WORK_ORDER_TRANSITIONS } from '@/constants/workOrders';
import type { WorkOrder, WorkOrderStatus } from '@/types';

interface WorkOrderStatusActionsProps {
    workOrder: WorkOrder;
    onChanged: (workOrder: WorkOrder) => void;
}

function WorkOrderStatusActions({ workOrder, onChanged }: WorkOrderStatusActionsProps) {
    const [assignee, setAssignee] = useState(workOrder.assignee ?? '');
    const [error, setError] = useState<string>();
    const [saving, setSaving] = useState(false);
    const next = WORK_ORDER_TRANSITIONS[workOrder.status];

    const move = (status: WorkOrderStatus) => {
        setSaving(true);
        setError(undefined);
        const request = status === ASSIGNEE_REQUIRED_STATUS ? { status, assignee: assignee || null } : { status };
        workOrdersApi.updateStatus(workOrder.id, request)
            .then(onChanged)
            .catch((err: unknown) => setError(err instanceof Error ? err.message : API_ERROR_FALLBACK))
            .finally(() => setSaving(false));
    };

    if (next.length === 0) return <p className="muted">This work order is closed. No further actions.</p>;
    return (
        <div className="wo-status-actions">
            {next.includes(ASSIGNEE_REQUIRED_STATUS) && (
                <select className="input" value={assignee} onChange={e => setAssignee(e.target.value)}>
                    <option value="">Select engineer</option>
                    {FIELD_ENGINEERS.map(n => <option key={n} value={n}>{n}</option>)}
                </select>
            )}
            {next.map(status => (
                <button key={status} className={`wo-btn ${status === 'Cancelled' ? 'danger' : 'primary'}`} disabled={saving} onClick={() => move(status)}>
                    {status === ASSIGNEE_REQUIRED_STATUS ? 'Assign' : `Move to ${WORK_ORDER_STATUS_LABELS[status]}`}
                </button>
            ))}
            {error && <p className="alert-error">{error}</p>}
        </div>
    );
}

export default WorkOrderStatusActions;
