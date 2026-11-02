import { Link, useParams } from 'react-router-dom';
import { equipmentApi, healthApi, sitesApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { SiteEquipmentCard } from '@/components/equipment';
import { SiteHealthCard } from '@/components/health';
import { SiteInfoCard, TenanciesTable, TowerCapacityCard } from '@/components/sites';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import { ROUTES } from '@/constants/navigation';

function SiteDetailPage() {
    const siteId = Number(useParams().siteId);
    const { data: site, error, loading } = useApi(() => sitesApi.get(siteId), [siteId]);
    const equipment = useApi(() => equipmentApi.list({ siteId }), [siteId]);
    const health = useApi(() => healthApi.site(siteId), [siteId]);

    return (
        <>
            <Link to={ROUTES.sites} className="back-link">← Back to sites</Link>
            <PageHeader title={site?.name ?? 'Site'} subtitle="Site, tower, capacity and tenancies" />
            <StateMessage loading={loading} error={error} />
            {site && (
                <>
                    <div className="grid-2"><SiteInfoCard site={site} /><TowerCapacityCard site={site} /></div>
                    <Card title="Tenancies">
                        <StateMessage empty={!site.tenancies.length} emptyText="No tenants on this tower yet." />
                        {!!site.tenancies.length && <TenanciesTable tenancies={site.tenancies} />}
                    </Card>
                    <StateMessage loading={equipment.loading} error={equipment.error} />
                    {equipment.data && <SiteEquipmentCard items={equipment.data} />}
                    {health.data && <SiteHealthCard health={health.data} />}
                </>
            )}
        </>
    );
}

export default SiteDetailPage;
