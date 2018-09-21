load("-text", "Convergencia.matrix", "data")

semilogx (data(:,1), data(:,2), '-r;Valor Call Black-Scholes;',...
	data(:,1), data(:,3), '-b;Valor Call CAPM;');
	
xlabel('Numero de Pasos');
ylabel("Valor del Call");
%title("Utilidad y Frontera de Eficiencia");
axis([2,7000,3,4.5,0,10],"autoz","ticxyz");
grid("on");

% SAVE THE FILES
print -color -dashes -dpng "convergencia.png";
