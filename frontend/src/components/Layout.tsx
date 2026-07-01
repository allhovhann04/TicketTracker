import { NavLink, Outlet } from 'react-router-dom'

export function Layout() {
  return (
    <div>
      <header>
        <nav>
          <NavLink to="/">Board</NavLink>
          <NavLink to="/teams">Teams</NavLink>
          <NavLink to="/epics">Epics</NavLink>
        </nav>
      </header>
      <main>
        <Outlet />
      </main>
    </div>
  )
}
