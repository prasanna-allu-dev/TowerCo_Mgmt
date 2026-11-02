import { useState } from 'react';
import { workOrdersApi } from '@/features/workOrders/api';
import { ASSIGNEE_REQUIRED_STATUS, FIELD_ENGINEERS, WORK_ORDER_STATUS_LABELS, WORK_ORDER_TRANSITIONS } from '@/features/workOrders/constants/workOrders';
import type { WorkOrder, WorkOrderStatus } from '@/features/workOrders/types';
import { SelectField } from '@/shared/components';
import { useAsyncAction } from '@/shared/hooks/useAsyncAction';

interface WorkOrderStatusActionsProps {
    workOrder: WorkOrder;
    onChanged: (workOrder: WorkOrder) => void;
}

const CANCELLED_STATUS: WorkOrderStatus = 'Cancelled';

const transitionLabel = (status: WorkOrderStatus) =>
    status === ASSIGNEE_REQUIRED_STATUS ? 'Assign' : `Move to ${WORK_ORDER_STATUS_LABELS[status]}`;

function WorkOrderStatusActions({ workOrder, onChanged }: WorkOrderStatusActionsProps) {
    const [assignee, setAssignee] = useState(workOrder.assignee ?? '');
    const save = useAsyncAction(workOrdersApi.updateStatus, onChanged);
    const next = WORK_ORDER_TRANSITIONS[workOrder.status];
    const move = (status: WorkOrderStatus) =>
        save.run(workOrder.id, status === ASSIGNEE_REQUIRED_STATUS ? { status, assignee: assignee || null } : { status });

    if (next.length === 0) return <p className="muted">This work order is closed. No further actions.</p>;
    return (
        <div className="wo-status-actions">
            {next.includes(ASSIGNEE_REQUIRED_STATUS) && (
                <SelectField label="Engineer" value={assignee} options={FIELD_ENGINEERS} placeholder="Select engineer" onChange={setAssignee} />
            )}
            {next.map(status => (
                <button key={status} className={`wo-btn ${status === CANCELLED_STATUS ? 'danger' : 'primary'}`}
                    disabled={save.pending} onClick={() => move(status)}>{transitionLabel(status)}</button>
            ))}
            {save.error && <p className="alert-error">{save.error}</p>}
        </div>
    );
}

export default WorkOrderStatusActions;
