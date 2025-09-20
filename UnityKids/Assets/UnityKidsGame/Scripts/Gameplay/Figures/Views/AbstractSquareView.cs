namespace UnityKids.Gameplay
{
    public abstract class AbstractSquareView : AbstractFigureView
    {
        private Square _square;
        public Square Square => _square;
        protected IFigureService _figureService;
        /// <summary>
        /// Размер кубов в переводе на Transform. ВАЖНО: Не реализовывал перерасчет от смены резрешения,
        /// поэтому если менять разрешение экрана в рантайме = получаем баг c отображением, так как этот параметр
        /// не рассчитался заного и будет некорректен для иного разрешения на котором был посчитан
        /// При сейв\лоаде также объекты встанут криво если сменить разрешение, фикс этого также не реализовывался.
        /// </summary>
        protected float _size;
        public virtual void Initialize(Square square, IFigureService figureService)
        {
            _square = square;
            _figureService = figureService;
        }
        

    }
}