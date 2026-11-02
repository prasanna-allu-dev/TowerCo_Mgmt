import { Card, StateMessage } from '@/shared/components';
import type { SiteInfrastructure, SiteInfrastructureKind } from '@/features/sites/types';

const KIND_LABELS: Record<SiteInfrastructureKind, string> = {
    Generator: 'Diesel generator',
    Battery: 'Battery bank',
    Solar: 'Solar array',
    Cooling: 'Cooling unit',
};

/** TowerCo power and cooling kit, kept apart from tenant equipment because it is never leased. */
function SiteInfrastructureCard({ items }: { items: SiteInfrastructure[] }) {
    return (
        <Card title="TowerCo site infrastructure">
            <StateMessage empty={!items.length} emptyText="No TowerCo infrastructure recorded." />
            {!!items.length && (
                <table className="table">
                    <thead><tr><th>Kind</th><th>Serial</th><th>Installed</th></tr></thead>
                    <tbody>
                        {items.map(item => (
                            <tr key={item.id}>
                                <td>{KIND_LABELS[item.kind]}</td>
                                <td className="mono">{item.serialNumber}</td>
                                <td className="mono">{item.installedOn}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </Card>
    );
}

export default SiteInfrastructureCard;
