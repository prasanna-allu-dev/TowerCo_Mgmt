import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';

function AppLayout() {
    return (
        <div className="shell">
            <Sidebar />
            <main className="content">
                <Outlet />
            </main>
        </div>
    );
}

export default AppLayout;
