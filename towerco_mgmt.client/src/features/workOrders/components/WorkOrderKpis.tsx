import { KpiTile } from '@/shared/components';
import type { WorkOrder } from '@/features/workOrders/types';
import { countSeverity } from '@/shared/utils/severity';
import { isClosed, isDueSoon, isOverdue } from '@/features/workOrders/utils/workOrder';

function WorkOrderKpis({ workOrders }: { workOrders: WorkOrder[] }) {
    const open = workOrders.filter(w => !isClosed(w));
    const overdue = open.filter(isOverdue).length;
    const dueSoon = open.filter(isDueSoon).length;
    const unassigned = open.filter(w => !w.assignee).length;
    return (
        <div className="kpi-row">
            <KpiTile label="Open work orders" value={open.length} hint={`${workOrders.length} in view`} />
            <KpiTile label="Overdue" value={overdue} tone={countSeverity(overdue, 'critical')} />
            <KpiTile label="Due this week" value={dueSoon} tone={countSeverity(dueSoon, 'warning')} />
            <KpiTile label="Unassigned" value={unassigned} tone={countSeverity(unassigned, 'warning')} />
        </div>
    );
}

export default WorkOrderKpis;
