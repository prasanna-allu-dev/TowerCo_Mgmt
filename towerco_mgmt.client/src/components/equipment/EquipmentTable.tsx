import { Link } from 'react-router-dom';
import SeverityPill from '@/components/dashboard/SeverityPill';
import { SHARED_OWNER_LABEL } from '@/constants/domain';
import type { EquipmentItem, EquipmentStatus, Severity } from '@/types';

const STATUS_SEVERITY: Record<EquipmentStatus, Severity> = { Installed: 'ok', Planned: 'warning', Removed: 'neutral' };

interface EquipmentTableProps {
    items: EquipmentItem[];
    showSite?: boolean;
}

function EquipmentRow({ item, showSite }: { item: EquipmentItem; showSite: boolean }) {
    return (
        <tr>
            {showSite && <td><Link to={`/sites/${item.siteId}`} className="mono">{item.siteCode}</Link></td>}
            <td>{item.typeName}</td>
            <td>{item.category}</td>
            <td>{item.ownerName ?? <span className="muted">{SHARED_OWNER_LABEL}</span>}</td>
            <td className="mono">{item.serialNumber}</td>
            <td className="mono">{item.installedOn}</td>
            <td>{item.mountHeightMetres === null ? '-' : `${item.mountHeightMetres} m`}</td>
            <td><SeverityPill severity={STATUS_SEVERITY[item.status]} label={item.status} /></td>
        </tr>
    );
}

function EquipmentTable({ items, showSite = false }: EquipmentTableProps) {
    return (
        <table className="table">
            <thead>
                <tr>
                    {showSite && <th>Site</th>}
                    <th>Type</th><th>Category</th><th>Owner</th><th>Serial</th><th>Installed</th><th>Mount height</th><th>Status</th>
                </tr>
            </thead>
            <tbody>{items.map(item => <EquipmentRow key={item.id} item={item} showSite={showSite} />)}</tbody>
        </table>
    );
}

export default EquipmentTable;
