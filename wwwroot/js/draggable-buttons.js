document.addEventListener('DOMContentLoaded', function() {
    const draggables = document.querySelectorAll('.draggable-floating-btn');
    
    draggables.forEach(elm => {
        let isDragging = false;
        let startX, startY, initialLeft, initialTop;
        let hasMoved = false;

        // Mouse events
        elm.addEventListener('mousedown', dragStart);
        
        // Touch events
        elm.addEventListener('touchstart', dragStart, {passive: false});

        function dragStart(e) {
            if (e.type === 'mousedown' && e.button !== 0) return; // Only left click

            const clientX = e.type === 'touchstart' ? e.touches[0].clientX : e.clientX;
            const clientY = e.type === 'touchstart' ? e.touches[0].clientY : e.clientY;

            startX = clientX;
            startY = clientY;

            // Get current position
            const rect = elm.getBoundingClientRect();
            
            // We need to convert the current fixed position to top/left values if they aren't set yet
            // or if they are set to bottom/right in CSS
            
            // Set initial style to fixed if not already (it should be from CSS)
            elm.style.position = 'fixed';
            
            // Calculate current left/top based on rect
            initialLeft = rect.left;
            initialTop = rect.top;
            
            // Remove bottom/right constraints so we can control via top/left
            elm.style.bottom = 'auto';
            elm.style.right = 'auto';
            elm.style.left = initialLeft + 'px';
            elm.style.top = initialTop + 'px';

            isDragging = true;
            hasMoved = false;
            
            elm.style.cursor = 'grabbing';
            
            // Add global listeners
            document.addEventListener('mousemove', drag);
            document.addEventListener('mouseup', dragEnd);
            document.addEventListener('touchmove', drag, {passive: false});
            document.addEventListener('touchend', dragEnd);
        }

        function drag(e) {
            if (!isDragging) return;
            e.preventDefault(); // Prevent scrolling on touch

            const clientX = e.type === 'touchmove' ? e.touches[0].clientX : e.clientX;
            const clientY = e.type === 'touchmove' ? e.touches[0].clientY : e.clientY;

            const dx = clientX - startX;
            const dy = clientY - startY;

            if (Math.abs(dx) > 5 || Math.abs(dy) > 5) {
                hasMoved = true;
            }

            let newLeft = initialLeft + dx;
            let newTop = initialTop + dy;

            // Boundary checks (keep within window)
            const windowWidth = window.innerWidth;
            const windowHeight = window.innerHeight;
            const elmWidth = elm.offsetWidth;
            const elmHeight = elm.offsetHeight;

            if (newLeft < 0) newLeft = 0;
            if (newLeft + elmWidth > windowWidth) newLeft = windowWidth - elmWidth;
            if (newTop < 0) newTop = 0;
            if (newTop + elmHeight > windowHeight) newTop = windowHeight - elmHeight;

            elm.style.left = newLeft + 'px';
            elm.style.top = newTop + 'px';
        }

        function dragEnd(e) {
            if (!isDragging) return;
            isDragging = false;
            elm.style.cursor = 'grab';

            document.removeEventListener('mousemove', drag);
            document.removeEventListener('mouseup', dragEnd);
            document.removeEventListener('touchmove', drag);
            document.removeEventListener('touchend', dragEnd);

            // If we moved significantly, prevent the click on the child link
            if (hasMoved) {
                const preventClick = (clickEvent) => {
                    clickEvent.preventDefault();
                    clickEvent.stopPropagation();
                    elm.removeEventListener('click', preventClick, true);
                };
                elm.addEventListener('click', preventClick, true);
                
                // Also try to prevent click on children
                const links = elm.querySelectorAll('a');
                links.forEach(link => {
                    const preventLinkClick = (linkEvent) => {
                        linkEvent.preventDefault();
                        linkEvent.stopPropagation();
                        link.removeEventListener('click', preventLinkClick, true);
                    };
                    link.addEventListener('click', preventLinkClick, true);
                });
            }
        }
    });
});
