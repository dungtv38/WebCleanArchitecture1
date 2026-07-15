
export default function Footer() {
  return (
    <footer className="mt-12 bg-slate-50 border-t">
      <div className="max-w-6xl mx-auto px-4 py-6 flex items-center justify-between text-sm text-slate-600">
        <div>© {new Date().getFullYear()} TravelExplorer</div>
        <div className="flex gap-4">
          <a href="#privacy" className="hover:text-slate-900">Privacy</a>
          <a href="#terms" className="hover:text-slate-900">Terms</a>
        </div>
      </div>
    </footer>
  );
}