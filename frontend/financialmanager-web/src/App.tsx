import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AppLayout } from "./components/Layout/AppLayout";
import DashboardPage from "./pages/Dashboard/DashboardPage";
import PessoasPage from "./pages/pessoas/PessoasPage";
import CategoriasPage from "./pages/categorias/CategoriasPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/pessoas" element={<PessoasPage />} />
          <Route path="/categorias" element={<CategoriasPage />} />
          <Route path="/transacoes" element={<div style={{ color: "#fff" }}>Transações (em breve)</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
