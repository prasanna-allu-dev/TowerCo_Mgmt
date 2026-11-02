import { NAV_GROUPS } from '@/app/navigation';
import { countNavItems } from '@/app/personaNav';
import { PERSONAS, formatPageAccess } from '@/app/personas';
import type { Persona, PersonaId } from '@/app/personas';
import { CURRENT_USER } from '@/app/currentUser';
import UserAvatar from './UserAvatar';
import { PERSONA_ELEMENT_IDS } from './usePersonaMenu';
import type { PersonaMenu } from './usePersonaMenu';

const { label: LABEL_ID, trigger: TRIGGER_ID, list: LIST_ID } = PERSONA_ELEMENT_IDS;
const optionId = (id: PersonaId) => `persona-option-${id}`;
const pageAccess = (id: PersonaId) => formatPageAccess(countNavItems(NAV_GROUPS, id));

function PersonaAvatar({ persona }: { persona: Persona }) {
    return <span className={`persona-avatar persona-avatar--${persona.id}`} aria-hidden="true">{persona.icon}</span>;
}

export function PersonaTrigger({ persona, menu }: { persona: Persona; menu: PersonaMenu }) {
    return (
        <button id={TRIGGER_ID} type="button" className="persona-trigger" onClick={menu.toggle}
            aria-haspopup="listbox" aria-expanded={menu.isOpen} aria-controls={LIST_ID} aria-labelledby={`${LABEL_ID} ${TRIGGER_ID}`}>
            <UserAvatar />
            <span className="persona-text">
                <span className="persona-name">{CURRENT_USER.name}</span>
                <span className="persona-meta">{persona.icon} {persona.label}</span>
            </span>
            <span className={`persona-chevron${menu.isOpen ? ' open' : ''}`} aria-hidden="true">▾</span>
        </button>
    );
}

interface OptionProps {
    persona: Persona;
    isSelected: boolean;
    isActive: boolean;
    onHover: () => void;
    onChoose: () => void;
}

function PersonaOption({ persona, isSelected, isActive, onHover, onChoose }: OptionProps) {
    const className = `persona-option${isActive ? ' active' : ''}${isSelected ? ' selected' : ''}`;
    return (
        <li id={optionId(persona.id)} role="option" aria-selected={isSelected} className={className} onMouseEnter={onHover} onClick={onChoose}>
            <PersonaAvatar persona={persona} />
            <span className="persona-text">
                <span className="persona-name">{persona.label}</span>
                <span className="persona-description">{persona.description}</span>
                <span className="persona-meta">{pageAccess(persona.id)}</span>
            </span>
            <span className="persona-check" aria-hidden="true">{isSelected ? '✓' : ''}</span>
        </li>
    );
}

export function PersonaList({ selectedId, menu }: { selectedId: PersonaId; menu: PersonaMenu }) {
    return (
        <ul id={LIST_ID} className="persona-menu" role="listbox" tabIndex={-1} onKeyDown={menu.onKeyDown}
            aria-labelledby={LABEL_ID} aria-activedescendant={optionId(PERSONAS[menu.activeIndex].id)}>
            {PERSONAS.map((persona, index) => (
                <PersonaOption key={persona.id} persona={persona} isSelected={persona.id === selectedId} isActive={index === menu.activeIndex}
                    onHover={() => menu.setActiveIndex(index)} onChoose={() => menu.choose(index)} />
            ))}
        </ul>
    );
}
