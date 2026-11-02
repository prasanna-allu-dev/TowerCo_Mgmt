import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import AppLayout from '@/layout/AppLayout';
import SitesPage from '@/pages/SitesPage';
import SiteDetailPage from '@/pages/SiteDetailPage';
import OperatorsPage from '@/pages/OperatorsPage';
import ConfigurationPage from '@/pages/ConfigurationPage';
import CreateTowerSitePage from '@/pages/CreateTowerSitePage';
import DashboardPage from '@/pages/DashboardPage';
import TenantsPage from '@/pages/TenantsPage';
import EquipmentPage from '@/pages/EquipmentPage';
import WorkOrdersPage from '@/pages/WorkOrdersPage';
import WorkOrderDetailPage from '@/pages/WorkOrderDetailPage';
import HealthPage from '@/pages/HealthPage';
import { ROUTES } from '@/constants/navigation';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<AppLayout />}>
                    <Route path={ROUTES.sites} element={<SitesPage />} />
                    <Route path={ROUTES.siteDetail} element={<SiteDetailPage />} />
                    <Route path={ROUTES.operators} element={<OperatorsPage />} />
                    <Route path={ROUTES.tenants} element={<TenantsPage />} />
                    <Route path={ROUTES.equipment} element={<EquipmentPage />} />
                    <Route path={ROUTES.dashboard} element={<DashboardPage />} />
                    <Route path={ROUTES.configuration} element={<ConfigurationPage />} />
                    <Route path={ROUTES.newTowerSite} element={<CreateTowerSitePage />} />
                    <Route path={ROUTES.workOrders} element={<WorkOrdersPage />} />
                    <Route path={ROUTES.workOrderDetail} element={<WorkOrderDetailPage />} />
                    <Route path={ROUTES.health} element={<HealthPage />} />
                    <Route path="*" element={<Navigate to={ROUTES.sites} replace />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
