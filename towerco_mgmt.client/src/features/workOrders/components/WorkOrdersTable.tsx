import { useNavigate } from 'react-router-dom';
import { workOrderPath } from '@/shared/constants/routes';
import type { WorkOrder } from '@/features/workOrders/types';
import { DuePill, PriorityPill, WorkOrderStatusPill } from './WorkOrderPills';

function WorkOrdersTable({ workOrders }: { workOrders: WorkOrder[] }) {
    const navigate = useNavigate();
    return (
        <table className="table">
            <thead>
                <tr><th>Code</th><th>Title</th><th>Site</th><th>Priority</th><th>Status</th><th>Assignee</th><th>Due</th></tr>
            </thead>
            <tbody>
                {workOrders.map(wo => (
                    <tr key={wo.id} className="clickable" onClick={() => navigate(workOrderPath(wo.id))}>
                        <td className="mono">{wo.code}</td>
                        <td>{wo.title}<div className="muted">{wo.category}</div></td>
                        <td><span className="mono">{wo.siteCode}</span><div className="muted">{wo.siteName}</div></td>
                        <td><PriorityPill priority={wo.priority} /></td>
                        <td><WorkOrderStatusPill status={wo.status} /></td>
                        <td>{wo.assignee ?? <span className="muted">Unassigned</span>}</td>
                        <td><DuePill workOrder={wo} /></td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default WorkOrdersTable;
