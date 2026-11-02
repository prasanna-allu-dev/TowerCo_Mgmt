import { NavLink } from 'react-router-dom';
import { APP_NAME, APP_SUBTITLE, NAV_GROUPS } from '@/app/navigation';
import type { NavGroup, NavItem } from '@/app/navigation';
import { filterNavGroups } from '@/app/personaNav';
import { usePersona } from '@/app/usePersona';
import { useSidebarExpanded } from './useSidebarExpanded';

function NavEntry({ item }: { item: NavItem }) {
    const content = <><span className="nav-icon" aria-hidden="true">{item.icon}</span><span className="nav-text">{item.label}</span></>;
    if (item.disabled) return <span className="nav-link disabled" title={`${item.label} (coming soon)`}>{content}</span>;
    return <NavLink to={item.path} className="nav-link" title={item.label}>{content}</NavLink>;
}

function NavSection({ group }: { group: NavGroup }) {
    return (
        <div className="nav-group" role="group" aria-label={group.label}>
            <div className="nav-group-label">{group.label}</div>
            {group.items.map(item => <NavEntry key={item.path} item={item} />)}
        </div>
    );
}

function SidebarToggle({ expanded, onToggle }: { expanded: boolean; onToggle: () => void }) {
    const label = expanded ? 'Collapse menu' : 'Expand menu';
    return (
        <button type="button" className="sidebar-toggle" onClick={onToggle} aria-expanded={expanded} aria-label={label} title={label}>
            <span aria-hidden="true">{expanded ? '«' : '»'}</span>
        </button>
    );
}

function Sidebar() {
    const { persona } = usePersona();
    const { expanded, toggle } = useSidebarExpanded();
    return (
        <aside className={`sidebar${expanded ? ' expanded' : ''}`}>
            <div className="brand" title={`${APP_NAME} ${APP_SUBTITLE}`}>
                <span className="brand-mark" aria-hidden="true">T</span>
                <div className="brand-text"><div className="brand-name">{APP_NAME}</div><div className="brand-sub">{APP_SUBTITLE}</div></div>
            </div>
            <SidebarToggle expanded={expanded} onToggle={toggle} />
            <nav className="nav">{filterNavGroups(NAV_GROUPS, persona).map(g => <NavSection key={g.label} group={g} />)}</nav>
        </aside>
    );
}

export default Sidebar;
